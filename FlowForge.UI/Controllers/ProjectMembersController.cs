using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class ProjectMembersController(UserManager<ApplicationUser> userManager, IProjectService projectService, IProjectMemberService projectMemberService, ITaskService taskService) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IProjectService _projectService = projectService;
        private readonly IProjectMemberService _projectMemberService = projectMemberService;
        private readonly ITaskService _taskService = taskService;

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> JoinRequest(Guid projectId)
        {
            return View(new ProjectJoinRequest { ProjectId = projectId });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> JoinRequest(ProjectJoinRequest JoinRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors)
                    .Select(m => m.ErrorMessage);
                return View(JoinRequest);
            }
            var userToAdd = await _userManager.FindByEmailAsync(JoinRequest.UserEmail);
            if (userToAdd == null)
            {
                ModelState.AddModelError("UserEmail", "User not found");
                return View(JoinRequest);
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            var project = await _projectService.GetProjectById(user.Id, JoinRequest.ProjectId);

            if (project == null)
            {
                ModelState.AddModelError("projectId", "Project not found");
                return View(JoinRequest);
            }
            JoinRequest.AddedUserId = userToAdd.Id;
            JoinRequest.AddedUser = userToAdd;

            JoinRequest.AddingUserId = user.Id;
            JoinRequest.AddingUser = user;

            bool result = await _projectMemberService.SendJoinRequest(JoinRequest);
            if (!result)
            {
                ModelState.AddModelError("UserEmail", "Failed to add user to project");
                return View(JoinRequest);
            }

            return RedirectToAction("Index", "Projects");
        }

        [HttpPost]
        [Route("[action]/{projectId:guid}")]
        public async Task<IActionResult> AcceptInvite(Guid projectId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                await _projectMemberService.AcceptProjectMember(projectId, user.Id);
                return RedirectToAction("Index", "Projects");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error accepting invite: {ex.Message}");
                return View("~/Views/Projects/Index.cshtml", await _projectService.GetProjects(user.Id));
            }
        }

        [HttpPost]
        [Route("[action]/{projectId:guid}")]
        public async Task<IActionResult> RejectInvite(Guid projectId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                await _projectMemberService.RejectProjectMember(projectId, user.Id);
                return RedirectToAction("Index", "Projects");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error rejecting invite: {ex.Message}");
                return View("~/Views/Projects/Index.cshtml", await _projectService.GetProjects(user.Id));
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RemoveMember(Guid projectId, Guid memberId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                await _projectMemberService.RemoveProjectMember(projectId, memberId, user.Id);
                return RedirectToAction("Tasks", "Tasks", new { projectId });
            }
            catch (Exception ex)
            {
                var sections = await _taskService.GetAllProjectTasks(user.Id, projectId);
                var members = await _projectService.GetProjectMembers(projectId);
                ViewBag.Members = members;
                ViewBag.ProjectId = projectId;
                ModelState.AddModelError("", $"Error removing member: {ex.Message}");

                return View("~/Views/Tasks/Tasks.cshtml", sections);
            }
        }
    }
}
