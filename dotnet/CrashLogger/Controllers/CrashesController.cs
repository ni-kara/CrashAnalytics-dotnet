using CrashAnalytics.Models;
using CrashLogger.Services;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Immutable;
using System.Diagnostics;

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

        private const int defaultIndexPage = 1;
        private const int defaultSizePage = 10;
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CrashDTO>>> GetCrashes1(Guid projectId, [FromQuery] int indexPage = defaultIndexPage, [FromQuery] int sizePage = defaultSizePage, Crash.DeviceType? type = null)
        {
            var projectQuery = _context.Projects
                .Where(p => p.Id == projectId);

            if (type.HasValue)
            {
                projectQuery = projectQuery
                    .Include(p => p.Crashes
                        .Where(p => p.Type == type)

                        .OrderByDescending((c) => c.CreatedAt)
                        .Skip((indexPage - 1) * sizePage)
                        .Take(sizePage)
                    );
            }
            else
            {
                projectQuery = projectQuery
                  .Include(p => p.Crashes
                    .OrderByDescending((c) => c.CreatedAt)
                    .Skip((indexPage - 1) * sizePage)
                    .Take(sizePage)
                  );
            }

            var project = await projectQuery.SingleOrDefaultAsync();

            if (project == null)
                return NotFound("Project does not exist");

            var crashQuery = _context.Crashes.Where(c => c.ProjectId == projectId);

            if (type.HasValue)
                crashQuery = crashQuery.Where(p=> p.Type == type);

            var totalCount = await crashQuery.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / sizePage);
           
            var result = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = indexPage,
                PageSize = project.Crashes.Count(),
                Crashes = project.Crashes.ToList(),
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
