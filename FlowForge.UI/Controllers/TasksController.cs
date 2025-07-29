using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowForge.UI.Controllers
{
    [Controller]
    //[Authorize]
    public class TasksController(IProjectService projectService, ITaskService taskService, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IProjectService _projectService = projectService;
        private readonly ITaskService _taskService = taskService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        [Route("projects/tasks")]
        public async Task<IActionResult> Tasks(Guid projectId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var tasks = await _taskService.GetAllProjectTasks(user.Id, projectId);
            ViewBag.projectId = projectId;
            if (tasks == null)
            {
                return BadRequest("Error");
            }

            return View(tasks);
        }

        [HttpGet]
        [Route("projects/tasks/new")]
        public IActionResult Task(Guid projectId, Guid sectionId)
        {
            ViewBag.projectId = projectId;
            ViewBag.sectionId = sectionId;
            return View("AddTask");
        }

        [HttpPost]
        [Route("projects/tasks/new")]
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
        [Route("projects/tasks/delete")]
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

            var taskToDelete = task.ToToDoItem();
            taskToDelete.CreatedById = user.Id;
            bool DeleteSuccess = await _taskService.DeleteTask(taskToDelete);
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
        [Route("projects/tasks/Edit")]
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
        [Route("projects/tasks/Edit")]
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
        [Route("taskState/{taskId}")]
        public async Task<IActionResult> State(Guid taskId)
        {
            if (taskId == Guid.Empty)
            {
                return BadRequest("Invalid request");
            }

            bool updated = await _taskService.CheckAsCompleted(taskId);

            if (!updated)
            {
                return BadRequest("Task not found or could not be updated");
            }

            return Ok(new { success = true, taskId });
        }
    }
}