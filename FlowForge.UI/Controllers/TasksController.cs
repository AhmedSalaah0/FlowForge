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
    public class TasksController(IProjectService projectService, ITaskService taskService, IProjectMemberService projectMemberService, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IProjectService _projectService = projectService;
        private readonly ITaskService _taskService = taskService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> Tasks(Guid projectId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var tasks = await _taskService.GetAllProjectTasks(user.Id, projectId);
            ViewBag.projectId = projectId;
            var members = await _projectService.GetProjectMembers(projectId);
            ViewBag.members = members;
            if (tasks == null)
            {
                return BadRequest("Error");
            }

            return View(tasks);
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
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                ModelState.AddModelError("Add Task", "User Not Found");
            }
            await _taskService.AddTask(user.Id, taskAddRequest);
            return RedirectToAction("Tasks", new
            {
                projectId = taskAddRequest.ProjectId
            });
        }

        [HttpGet]
        [Route("ScheduleTask")]
        public async Task<IActionResult> ScheduleTask(Guid taskId, Guid projectId)
        {
            var task = await _taskService.GetTaskById(projectId, taskId);
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
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(request);
            }
            request.MemberId = user.Id;

            bool scheduled = await _taskService.ScheduleTask(request);
            
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
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            bool unscheduled = await _taskService.UnScheduleTask(taskId, projectId, user.Id);
            
            if (!unscheduled)
            {
                ModelState.AddModelError("", "Failed to unschedule the task. Please try again.");
                return View("Tasks", await _taskService.GetAllProjectTasks(user.Id, projectId));
            }
            
            TempData["SuccessMessage"] = "Task unscheduled successfully!";
            
            return RedirectToAction("Tasks", new { projectId });
        }

        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId)
        {
            var task = await _taskService.GetTaskById(projectId, taskId);
            if (task == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            task.CreatedById = user.Id;
            bool DeleteSuccess = await _taskService.DeleteTask(task);
            if (!DeleteSuccess)
            {
                ModelState.AddModelError("Delete Task", "Task not found or could not be deleted");
                return View("SubTasks", await _taskService.GetAllProjectTasks(user.Id, projectId));
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
            var task = await _taskService.GetTaskById(projectId, taskId);
            if (task == null)
            {
                return BadRequest();
            }
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
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            taskUpdateRequest.MemberId = user.Id;
            await _taskService.UpdateTask(taskUpdateRequest);
            return RedirectToAction("Tasks", new
            {
                taskUpdateRequest.ProjectId
            });
        }

        [HttpGet]
        [Route("tasks/completed")]
        public async Task<IActionResult> CompletedTasks()
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var tasks = await _taskService.GetAllCompletedTask(user.Id);
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

            bool updated = await _taskService.UpdateTaskStatus(taskUpdateStatus);

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
            var task = await _taskService.GetTaskById(request.ProjectId, request.TaskId);
            if (task == null) return NotFound();

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            await _taskService.MoveTask(user.Id, request);
            return Ok();
        }
    }
}