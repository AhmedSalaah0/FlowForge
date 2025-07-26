using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using FlowForge.UI.Filters.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]")]
    [Authorize]
    public class ProjectsController(IProjectService projectService, INotificationService notificationService, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IProjectService _projectService = projectService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly INotificationService _notificationService = notificationService;

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tasks = await _projectService.GetProjects(user.Id);

            var Notifications = await _notificationService.GetNotifications(user.Id);
            ViewBag.Notifications = Notifications;
             if (Tasks == null)
            {
                return BadRequest("Error");
            }

            return View(Tasks);
        }
        
        [HttpGet]
        [Route("projects")]
        [TypeFilter(typeof(SetColorOptionsFilter))]
        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        [Route("projects")]
        public async Task<IActionResult> CreateProject(ProjectAddRequest projectAddRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            await _projectService.CreateProject(projectAddRequest, user.Id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("projects/Edit")]
        [TypeFilter(typeof(SetColorOptionsFilter))]

        public async Task<IActionResult> EditProject(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return BadRequest("Invalid project ID");
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }    
            var project = await _projectService.GetProjectById(user.Id ,projectId);
            if (project == null)
            {
                return BadRequest();
            }
            return View("EditProject",project);
        }

        [HttpPost]
        [Route("projects/Edit")]
        public async Task<IActionResult> EditProject(Guid projectId, ProjectUpdateRequest projectUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors)
                    .Select(m => m.ErrorMessage);
                return View();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user == null)
            {
                return BadRequest();
            }
            projectUpdateRequest.UserId = user.Id;
            await _projectService.UpdateProject(projectId, projectUpdateRequest);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("projects/delete/{projectId}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user is null)
            {
                return BadRequest();
            }    
            var project = await _projectService.GetProjectById(user.Id, projectId);
            if (project == null)
            {
                return BadRequest();
            }
            bool result = await _projectService.DeleteProject(user.Id, projectId);
            if (!result)
            {
                ModelState.AddModelError("ProjectId", "Failed to delete project");
                return View("Index", await _projectService.GetProjects(user.Id));
            }
            return RedirectToAction("Index");
        }
    }
}