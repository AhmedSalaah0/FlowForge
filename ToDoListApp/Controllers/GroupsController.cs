using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ToDoListApp.Filters.ActionFilters;
namespace ToDoListApp.Controllers
{
    [Controller]
    public class GroupsController(IGroupService groupService, ITaskService taskService) : Controller
    {
        private readonly IGroupService _groupService = groupService;
        private readonly ITaskService _taskService = taskService;

        [Route("groups")]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var Tasks = await _groupService.GetGroups();
            if (Tasks == null)
            {
                return BadRequest("Error");
            }

            return View(Tasks);
        }
        
        [HttpGet]
        [Route("groups/new")]
        [TypeFilter(typeof(SetColorOptionsFilter))]
        public IActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        [Route("groups/new")]
        public async Task<IActionResult> AddGroup(GroupAddRequest groupAddRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _groupService.CreateGroup(groupAddRequest);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("groups/delete/{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            var group = await _groupService.GetGroupById(groupId);
            if (group == null)
            {
                return BadRequest();
            }
            await _groupService.DeleteGroup(groupId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("groups/Edit")]
        [TypeFilter(typeof(SetColorOptionsFilter))]

        public async Task<IActionResult> EditGroup(Guid groupId)
        {
            var group = await _groupService.GetGroupById(groupId);
            if (group == null)
            {
                return BadRequest();
            }
            return View(group);
        }

        [HttpPost]
        [Route("groups/Edit")]
        public async Task<IActionResult> EditGroup(Guid groupId, GroupUpdateRequest groupUpdateRequest)
        {
            await _groupService.UpdateGroup(groupId, groupUpdateRequest);
            return RedirectToAction("Index");
        }
    }
}