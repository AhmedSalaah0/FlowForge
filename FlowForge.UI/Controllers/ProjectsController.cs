using FlowForge.Core.Domain.Entities;
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
    public class ProjectsController(IProjectService projectService, ITaskService taskService, INotificationService notificationService, UserManager<ApplicationUser> userManager) : Controller
    {
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
            var Tasks = await projectService.GetProjects(user.Id);

            var Notifications = await notificationService.GetNotifications(user.Id);
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
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            projectAddRequest.CreatedById = user.Id;
            await projectService.CreateProject(projectAddRequest);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("projects/Edit")]
        [TypeFilter(typeof(SetColorOptionsFilter))]

        public async Task<IActionResult> EditProject(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                ModelState.AddModelError("ProjectId", "Project ID cannot be empty");
            }
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }    
            var project = await projectService.GetProjectById(user.Id, projectId);
            if (project == null)
            {
                ModelState.AddModelError("ProjectId", "Project not found");
                return View("Index", await projectService.GetProjects(user.Id));
            }

            if (project.ProjectMembers.FirstOrDefault(u => u.MemberId == user.Id).MemberRole != Core.Enums.ProjectRole.Creator)
            {
                ModelState.AddModelError("ProjectId", "You do not have permission to edit this project");
                return View("Index", await projectService.GetProjects(user.Id));
            }
            return View("EditProject", project);
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
            var user = await userManager.FindByEmailAsync(User.Identity?.Name);
            if (user == null)
            {
                return BadRequest();
            }
            projectUpdateRequest.UserId = user.Id;
            await projectService.UpdateProject(projectId, projectUpdateRequest);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("delete/{projectId}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user is null)
            {
                return BadRequest();
            }    
            var project = await projectService.GetProjectById(user.Id, projectId);
            if (project == null)
            {
                return BadRequest();
            }
            bool result = await projectService.DeleteProject(user.Id, projectId);
            if (!result)
            {
                ModelState.AddModelError("ProjectId", "Failed to delete project");
                return View("Index", await projectService.GetProjects(user.Id));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> JoinProject(Guid projectId)
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user is null)
            {
                return BadRequest();
            }
            var result = projectService.AddProjectMember(new ProjectMemberAddRequest
            {
                ProjectId = projectId,
                MemberId = user.Id,
                MemberRole = Core.Enums.ProjectRole.Member
            });
            if (!result.Result)
            {
                ModelState.AddModelError("ProjectId", "Failed to join project");
                return View("Index", await projectService.GetProjects(user.Id));
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("change-visibility")]
        public async Task<IActionResult> ChangeVisibility(ChangeVisibilityRequest ChangeVisibilityRequest)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors)
                    .Select(m => m.ErrorMessage);
                ViewBag.ProjectId = ChangeVisibilityRequest.ProjectId;
                return View("~/Views/Tasks/Tasks.cshtml", await taskService.GetAllProjectTasks(user.Id, ChangeVisibilityRequest.ProjectId));
            }

            try
            {
                ChangeVisibilityRequest.UserId = user.Id;
                await projectService.ChangeVisibility(ChangeVisibilityRequest);
                TempData["Visibility"] = ChangeVisibilityRequest.ProjectVisibility;
                return RedirectToAction("Tasks", "Tasks", new
                {
                    projectId = ChangeVisibilityRequest.ProjectId
                });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while changing project visibility.");
            }
            var project = await projectService.GetProjectById(user.Id, ChangeVisibilityRequest.ProjectId);
            TempData["Visibility"] = project.ProjectVisibility;
            ViewBag.ProjectId = ChangeVisibilityRequest.ProjectId;

            return View("~/Views/Tasks/Tasks.cshtml", await taskService.GetAllProjectTasks(user.Id, ChangeVisibilityRequest.ProjectId));
        }
    }
}