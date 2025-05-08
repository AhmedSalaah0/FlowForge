using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ToDoListApp.Controllers
{
    [Controller]
    public class TasksController(IGroupService groupService, ITaskService taskService) : Controller
    {
        private readonly IGroupService _groupService = groupService;
        private readonly ITaskService _taskService = taskService;

        [HttpGet]
        [Route("groups/tasks")]
        public async Task<IActionResult> SubTasks(Guid groupId)
        {
            var tasks = await _taskService.GetAllGroupTasks(groupId);
            ViewBag.groupId = groupId;
            if (tasks == null)
            {
                return BadRequest("Error");
            }
            return View(tasks);
        }

        [HttpGet]
        [Route("groups/tasks/new")]
        public IActionResult AddSubTask(Guid groupId)
        {
            ViewBag.groupId = groupId;
            return View();
        }

        [HttpPost]
        [Route("groups/tasks/new")]
        public async Task<IActionResult> AddSubTask(TaskAddRequest taskAddRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _taskService.AddTask(taskAddRequest);
            return RedirectToAction("SubTasks", new
            {
                groupId = taskAddRequest.GroupId
            });
        }

        [HttpGet]
        [Route("groups/tasks/delete")]
        public async Task<IActionResult> DeleteTask(Guid groupId, Guid taskId)
        {
            var task = await _taskService.GetTaskById(groupId, taskId);
            if (task == null)
            {
                return BadRequest();
            }

            bool DeleteSuccess = await _taskService.DeleteTask(task.ToToDoItem());
            if (!DeleteSuccess)
            {
                return BadRequest();
            }

            if (Request.Headers["Referer"].ToString().Contains("completed"))
            {
                return RedirectToAction("CompletedTasks");
            }
            return RedirectToAction("SubTasks", new
            {
                groupId
            });
        }

        [HttpGet]
        [Route("groups/tasks/Edit")]
        public async Task<IActionResult> EditTask(Guid groupId, Guid taskId)
        {
            var task = await _taskService.GetTaskById(groupId, taskId);
            if (task == null)
            {
                return BadRequest();
            }
            return View(task);
        }

        [HttpPost]
        [Route("groups/tasks/Edit")]
        public async Task<IActionResult> EditTask(TaskUpdateRequest taskUpdateRequest)
        {
            await _taskService.UpdateTask(taskUpdateRequest);
            return RedirectToAction("SubTasks", new
            {
                taskUpdateRequest.GroupId
            });
        }

        [HttpGet]
        [Route("tasks/completed")]
        public async Task<IActionResult> CompletedTasks()
        {
            var tasks = await _taskService.GetAllCompletedTask();
            return View(tasks);
        }

        [HttpPost]
        [Route("taskState/{taskId}")]
        public async Task<IActionResult> ChangeState(Guid taskId)
        {
            if (taskId == Guid.Empty)
            {
                return BadRequest("Invalid request");
            }

            bool updated = await _taskService.CheckAsCompleted(taskId);

            if (!updated)
            {
                string s = ":ss", ss ="";

                string c = s.Select(t => ss.Contains(t) == false).ToString();
                return BadRequest("Task not found or could not be updated");
            }

            return Ok(new { success = true, taskId });
        }
    }
}