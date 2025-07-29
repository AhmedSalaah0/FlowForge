using FlowForge.Core.Domain.Entities;
using FlowForge.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.UI.Controllers
{
    [ApiController]
    [Authorize("Admin")]
    [Route("Admin")]
    public class AdminController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        [HttpPost]
        [Route("MigrateTasksToSections")]
        public async Task<IActionResult> MigrateTasksToSections()
        {
            var projects = await _context.Projects.ToListAsync();

            foreach (var project in projects)
            {
                var section = new ProjectSection
                {
                    SectionId = Guid.NewGuid(),
                    SectionName = "General",
                    ProjectId = project.ProjectId,
                    CreatedById = project.CreatedById
                };
                _context.Sections.Add(section);

                var oldTasks = await _context.Tasks
                    .Where(t => t.ProjectId == project.ProjectId && t.SectionId == null)
                    .ToListAsync();

                foreach (var task in oldTasks)
                {
                    task.SectionId = section.SectionId;
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Migration completed successfully.");
        }
    }
}
