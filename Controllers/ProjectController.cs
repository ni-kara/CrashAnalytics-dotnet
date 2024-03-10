using CrashAnalytics.Models;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace CrashAnalytics.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController: ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProjectController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            var projects = await _context.Projects.Select(p => new { p.Id, p.Name, p.CreatedAt }).ToListAsync();

            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById(Guid projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Crashes)
                .SingleOrDefaultAsync(s => s.Id.Equals(projectId));

            if (project == null)
                return NotFound("The project ID does not exist");

            return project;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> PostProject(Project project)
        {
            ProjectDTO projectDTO = new ProjectDTO();
            projectDTO.Name = project.Name; 

            _context.Projects.Add(projectDTO);

            await _context.SaveChangesAsync(); 

            return CreatedAtAction(nameof(GetProjectById), new { id = projectDTO.Id }, projectDTO);
        }      

        [HttpPut("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> PutProject(Guid projectId, Project project)
        {
            if (project == null)
                return BadRequest();

            var projectDTO = await _context.Projects.FindAsync(projectId);

            if (projectDTO == null)
                return NotFound("The project does not exist");

            projectDTO.Name = project.Name;            

            _context.Entry(projectDTO).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Accepted();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            
            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
