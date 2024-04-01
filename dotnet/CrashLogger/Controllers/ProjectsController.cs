using CrashAnalytics.Models;
using CrashLogger.Services;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Diagnostics;
using System.Xml.Linq;

namespace CrashAnalytics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ApplicationContext _context;

        public ProjectsController(ICacheService cacheService, ApplicationContext context)
        {
            _cacheService = cacheService;
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            var cachedData = _cacheService.GetData<IEnumerable<ProjectDTO>>("projects");

            if (cachedData != null)
                return Ok(cachedData);
            
            var projects = await _context.Projects.Select(p => new { p.Id, p.Name, p.CreatedAt }).ToListAsync();

            _cacheService.SetData("projects", projects, DateTimeOffset.Now.AddSeconds(10));
          
            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById(Guid projectId)
        {

            var cachedData = _cacheService.GetData<ProjectDTO>(projectId.ToString());

            if (cachedData != null)
                return Ok(cachedData);
            

            var project = await _context.Projects
                .Include(p => p.Crashes)
                .SingleOrDefaultAsync(s => s.Id.Equals(projectId));

            if (project == null)
                return NotFound("The project ID does not exist");

            _cacheService.SetData(projectId.ToString(), project, DateTimeOffset.Now.AddSeconds(10));

            Console.WriteLine("\nReturn: DB Data\n");
            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> PostProject(Project project)
        {
            if (project == null)
                return BadRequest();

            var p = await _context.Projects.FirstOrDefaultAsync(p => p.Name.Equals(project.Name));

            if (p != null)
                return UnprocessableEntity("The name of the project already exists");

            ProjectDTO projectDTO = new ProjectDTO();
            projectDTO.Name = project.Name;

            _context.Projects.Add(projectDTO);

            await _context.SaveChangesAsync();

            _cacheService.RemoveData("projects");

            return CreatedAtAction(nameof(GetProjectById), new { projectId = projectDTO.Id }, projectDTO);
        }

        [HttpPut("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> PutProject(Guid projectId, Project project)
        {
            if (project == null)
                return BadRequest();

            var projectWithUserName = await _context.Projects.FirstOrDefaultAsync(p => p.Name.Equals(project.Name));
            if (projectWithUserName != null)
                return Conflict("The name of the project already exists");

            var projectDTO = await _context.Projects.FindAsync(projectId);

            if (projectDTO == null)
                return NotFound("The project does not exist");

            projectDTO.Name = project.Name;

            _context.Entry(projectDTO).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            // Remove cached Projects
            _cacheService.RemoveData("projects");
            _cacheService.RemoveData(projectId.ToString());

            return AcceptedAtAction(nameof(GetProjectById), new { projectId = projectDTO.Id }, projectDTO);
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            // Remove cached Projects
            _cacheService.RemoveData("projects");
            _cacheService.RemoveData(projectId.ToString());

            return NoContent();
        }
    }
}
