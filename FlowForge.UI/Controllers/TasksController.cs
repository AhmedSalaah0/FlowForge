using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]")]
    //[Authorize]
    public class TasksController(IProjectService projectService, ITaskService taskService, IProjectMemberService projectMemberService, IReorderTaskService reorderTaskService, ILogger<TasksController> logger, UserManager<ApplicationUser> userManager) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Tasks(Guid projectId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            try
            {
                var tasks = await taskService.GetAllProjectTasks(user.Id, projectId);
                if (tasks == null)
                {
                    return BadRequest("Error");
                }

                ViewBag.projectId = projectId;
                var project = await projectService.GetProjectById(user.Id, projectId);
                var members = project.ProjectMembers
                    .Where(pm => pm.MembershipStatus == MembershipStatus.ACCEPTED)
                    .ToList();
                TempData["Visibility"] = project.ProjectVisibility;
                ViewBag.members = members;
                return View(tasks);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Join_Allowed")
                {
                    return View("~/Views/Projects/JoinProject.cshtml", projectId);
                }
                return Forbid();
            }   
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Task(Guid projectId, Guid sectionId)
        {
            ViewBag.projectId = projectId;
            ViewBag.sectionId = sectionId;
            return View("AddTask");
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Task(TaskAddRequest taskAddRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Add Task", "User Not Found");
            }
            await taskService.AddTask(user.Id, taskAddRequest);
            return RedirectToAction("Tasks", new
            {
                projectId = taskAddRequest.ProjectId
            });
        }

        [HttpGet]
        [Route("ScheduleTask")]
        public async Task<IActionResult> ScheduleTask(Guid taskId, Guid projectId)
        {
            var task = await taskService.GetTaskById(projectId, taskId);
            if (task == null)
            {
                ModelState.AddModelError("", "Task not found");
            }

            var scheduleRequest = new ScheduleTaskRequest
            {
                TaskId = taskId,
                ProjectId = projectId,
                ScheduleDate = DateOnly.FromDateTime(DateTime.Today),
                ScheduleTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(1))
            };

            return View(scheduleRequest);
        }

        [HttpPost]
        [Route("ScheduleTask")]
        public async Task<IActionResult> ScheduleTask(ScheduleTaskRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(request);
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(request);
            }
            request.MemberId = user.Id;

            bool scheduled = await taskService.ScheduleTask(request);

            if (!scheduled)
            {
                ModelState.AddModelError("", "Failed to schedule the task. Please try again.");
                return View(request);
            }

            TempData["SuccessMessage"] = "Task scheduled successfully!";

            return RedirectToAction("Tasks", new { projectId = request.ProjectId });
        }

        [HttpGet]
        [Route("UnScheduleTask")]
        public async Task<IActionResult> UnScheduleTask(Guid taskId, Guid projectId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            bool unscheduled = await taskService.UnScheduleTask(taskId, projectId, user.Id);

            if (!unscheduled)
            {
                ModelState.AddModelError("", "Failed to unschedule the task. Please try again.");
                return View("Tasks", await taskService.GetAllProjectTasks(user.Id, projectId));
            }

            TempData["SuccessMessage"] = "Task unscheduled successfully!";

            return RedirectToAction("Tasks", new { projectId });
        }

        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            bool DeleteSuccess = await taskService.DeleteTask(user.Id, projectId, taskId);
            if (!DeleteSuccess)
            {
                ModelState.AddModelError("Delete Task", "Task not found or could not be deleted");
                return View("Tasks", await taskService.GetAllProjectTasks(user.Id, projectId));
            }

            return RedirectToAction("Tasks", new
            {
                projectId
            });
        }

        [HttpGet]
        [Route("EditTask")]
        public async Task<IActionResult> EditTask(Guid projectId, Guid taskId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (projectId == Guid.Empty || taskId == Guid.Empty)
            {
                ModelState.AddModelError("ProjectId", "Project ID cannot be empty");
                return BadRequest(ModelState);
            }

            var project = await projectService.GetProjectById(user.Id, projectId);
            if (project == null)
            {
                ModelState.AddModelError("ProjectId", "Project not found");
                return BadRequest(ModelState);
            }

            if (project.ProjectMembers.FirstOrDefault(u => u.MemberId == user.Id)?.MemberRole == ProjectRole.Member)
            {
                ModelState.AddModelError("ProjectRole", "You do not have permission to edit this task");
                return View("Tasks", await taskService.GetAllProjectTasks(user.Id, projectId));
            }

            var task = await taskService.GetTaskById(projectId, taskId) ?? throw new KeyNotFoundException();
            return View(task);
        }

        [HttpPost]
        [Route("EditTask")]
        public async Task<IActionResult> EditTask(TaskUpdateRequest taskUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            taskUpdateRequest.MemberId = user.Id;
            await taskService.UpdateTask(taskUpdateRequest);
            return RedirectToAction("Tasks", new
            {
                taskUpdateRequest.ProjectId
            });
        }

        [HttpGet]
        [Route("tasks/completed")]
        public async Task<IActionResult> CompletedTasks()
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            var tasks = await taskService.GetAllCompletedTask(user.Id);
            return View(tasks);
        }

        [HttpPost]
        [Route("taskState")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] TaskUpdateStatus taskUpdateStatus)
        {
            if (taskUpdateStatus is null)
            {
                return BadRequest("Invalid request");
            }
            
            var task = await taskService.GetTaskById(taskUpdateStatus.ProjectId, taskUpdateStatus.TaskId);

            bool updated = await taskService.UpdateTaskStatus(taskUpdateStatus);

            if (!updated)
            {
                return BadRequest("Task not found or could not be updated");
            }

            return Ok(new { success = true, taskUpdateStatus.TaskId });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> MoveTask([FromBody] MoveTaskRequest request)
        {
            var task = await taskService.GetTaskById(request.ProjectId, request.TaskId);
            if (task == null) return NotFound();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            await taskService.MoveTask(user.Id, request);
            return Ok();
        }

        [HttpPost]
        [Route("Assign")]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskRequest assignRequest)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            assignRequest.UserId = user.Id;
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("assignTask","Can't Assign the task");
                return View("Tasks");
            }
            var task = await taskService.GetTaskById(assignRequest.ProjectId, assignRequest.TaskId);
            if (task == null)
            {
                return NotFound("Task not found");
            }
            var member = await projectService.GetProjectMember(assignRequest.ProjectId, assignRequest.MemberId);
            await taskService.AssignTask(assignRequest);
            return Ok("Task assigned successfully");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ReorderTasks([FromBody] ReorderRequest reorderRequest)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("ReorderTasks", "Invalid reorder request");
                return View("Tasks", await taskService.GetAllProjectTasks(user.Id, reorderRequest.ProjectId));
            }

            await reorderTaskService.ReorderTask(reorderRequest, user.Id);
            return Ok("Tasks reordered successfully");
        }
    }
}