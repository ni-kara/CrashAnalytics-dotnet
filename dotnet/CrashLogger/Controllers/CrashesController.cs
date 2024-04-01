using CrashAnalytics.Models;
using CrashLogger.Services;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Immutable;

namespace CrashAnalytics.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/[controller]")]
    public class CrashesController : ControllerBase
    {
        private readonly ICacheService _cacheService;   
        private readonly ApplicationContext _context;

        public CrashesController(ICacheService cacheService, ApplicationContext context)
        {
            _context = context;
            _cacheService = cacheService;   
        }

        [HttpPost()]
        public async Task<ActionResult<CrashDTO>> PostCrash(Guid projectId, Crash crash)
        {
            if (crash == null)
                return BadRequest("Invalid crash data");

            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound("Project does not exist");

            var crashDTO = new CrashDTO(crash);
            crashDTO.Project = project;
            crashDTO.ProjectId = project.Id;


            _context.Crashes.Add(crashDTO);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCrashById), new { projectId = projectId, crashId = crashDTO.Id }, crashDTO);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CrashDTO>>> GetCrashes(Guid projectId, [FromQuery] int indexPage = 1, [FromQuery] int sizePage = 10)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound("Project does not exist");

            var totalCount = await _context.Crashes
                .Where(c => c.ProjectId == projectId)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / sizePage);

            var crashes = await _context.Crashes
                .Where(c => c.ProjectId == projectId)
                .OrderByDescending(c => c.CreatedAt) 
                .Skip((indexPage - 1) * sizePage)
                .Take(sizePage)
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = indexPage,
                PageSize = crashes.Count(),
                Crashes = crashes.ToList(),
            };

            return Ok(result);
        }

        [HttpGet("{crashId}")]
        public async Task<ActionResult<IEnumerable<CrashDTO>>> GetCrashById(Guid projectId, Guid crashId)
        {
            var cachedData = _cacheService.GetData<ProjectDTO>(crashId.ToString());

            if (cachedData != null)
                return Ok(cachedData);

            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound("Project does not exist");

            var crash = await _context.Crashes
                .Include(p => p.Project)
                .Where(w => w.ProjectId.Equals(projectId) && w.Id.Equals(crashId))
                .SingleOrDefaultAsync();

            if (crash == null)
                return NotFound("Crash does not exist");

            _cacheService.SetData(crashId.ToString(), crash, DateTimeOffset.Now.AddSeconds(60));

            return Ok(crash);
        }



        [HttpPut("{crashId}")]
        public async Task<ActionResult<ProjectDTO>> PutCrash(Guid projectId, Guid crashId, Crash crash)
        {
            if (crash == null)
                return BadRequest("Invalid crash data");

            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound("Project does not exist");

            var crashDTO = await _context.Crashes.FindAsync(crashId);

            if (crashDTO == null)
                return NotFound("Project does not exist");

            crashDTO.Message = crash.Message;
            crashDTO.Version = crash.Version;
            crashDTO.Type = crash.Type;

            _context.Entry(crashDTO).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _cacheService.RemoveData(crashId.ToString());

            return AcceptedAtAction(nameof(GetCrashById), new { projectId = crashDTO.ProjectId, crashId = crashDTO.Id }, crashDTO);
        }


        [HttpDelete("{crashId}")]
        public async Task<ActionResult<ProjectDTO>> DeleteCrash(Guid projectId, Guid crashId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound("Project does not exist");

            var crashDTO = await _context.Crashes.FindAsync(crashId);

            if (crashDTO == null)
                return NotFound("Project does not exist");

            _context.Crashes.Remove(crashDTO);

            await _context.SaveChangesAsync();

            _cacheService.RemoveData(crashId.ToString());

            return NoContent();
        }

    }
}
