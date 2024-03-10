using CrashAnalytics.Models;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Immutable;

namespace CrashAnalytics.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class CrashController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CrashController(ApplicationContext context)
        {
            _context = context; 
        }

        [HttpGet("{projectId}/crashes")]
        public async Task<ActionResult<IEnumerable<CrashDTO>>> GetCrashes(Guid projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Crashes)
                .SingleOrDefaultAsync(p => p.Id.Equals(projectId));

            if (project == null)
                return NotFound("Project does not exist");

            return Ok(project.Crashes);
        }

        [HttpGet("{projectId}/crashes/{crashId}")]
        public async Task<ActionResult<IEnumerable<CrashDTO>>> GetCrashById(Guid projectId, Guid crashId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound("Project does not exist");

            var crash = await _context.Crashes
                .Include(p => p.Project)
                .Where(w=> w.ProjectId.Equals(projectId) && w.Id.Equals(crashId))
                .SingleOrDefaultAsync();

            if (crash == null)
                return NotFound("Crash does not exist");

            return Ok(crash);
        }

        [HttpPost("{projectId}/crash")]
        public async Task<ActionResult<ProjectDTO>> PostCrash(Guid projectId, Crash crash)
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

        [HttpPut("{projectId}/crashes/{crashId}")]
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

            return AcceptedAtAction(nameof(GetCrashById), new { projectId = crashDTO.ProjectId, crashId = crashDTO.Id }, crashDTO);
        }


        [HttpDelete("{projectId}/crashes/{crashId}")]
        public async Task<ActionResult<ProjectDTO>> DeleteCrash(Guid projectId, Guid crashId)
        {;

            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return NotFound("Project does not exist");

            var crashDTO = await _context.Crashes.FindAsync(crashId);

            if (crashDTO == null)
                return NotFound("Project does not exist");

            _context.Crashes.Remove(crashDTO);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
