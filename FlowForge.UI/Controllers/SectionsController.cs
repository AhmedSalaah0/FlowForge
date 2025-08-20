using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class SectionsController(UserManager<ApplicationUser> userManager, ISectionService sectionService) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ISectionService _sectionService = sectionService;
        [HttpPost]
        public async Task<IActionResult> AddSection(SectionAddRequest sectionAddRequest)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Add Section", "Invalid Model State");
                return RedirectToAction("Tasks", "Tasks");
            }

            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);

            if (user is null)
            {
                ModelState.AddModelError("User", "User not Found");
                return RedirectToAction("Tasks", "Tasks");
            }

            sectionAddRequest.CreatedBy = user;
            var result = await _sectionService.AddSection(user.Id, sectionAddRequest);
            if (result is null)
            {
                ModelState.AddModelError("Add Section", "Failed to add section");
            }
            return RedirectToAction("Tasks", "Tasks", new { projectId = sectionAddRequest.ProjectId});
        }

        [HttpGet("DeleteSection")]
        public async Task<IActionResult> DeleteSection(Guid sectionId, Guid projectId)
        {
            if (sectionId == Guid.Empty || projectId == Guid.Empty)
            {
                ModelState.AddModelError("Delete Section", "Invalid Section or Project ID");
                return RedirectToAction("Tasks", "Tasks", new { projectId });
            }
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user is null)
            {
                ModelState.AddModelError("User", "User not Found");
                return RedirectToAction("Tasks", "Tasks", new { projectId });
            }

            await _sectionService.DeleteSection(user.Id, sectionId);
            return RedirectToAction("Tasks", "Tasks", new { projectId });
        }
            
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditSectionName([FromBody] SectionUpdateRequest sectionUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Edit Section", "Invalid Section ID or Name");
                return RedirectToAction("Tasks", "Tasks");
            }
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user is null)
            {
                ModelState.AddModelError("User", "User not Found");
                return RedirectToAction("Tasks", "Tasks");
            }
            await _sectionService.EditSectionName(user.Id, sectionUpdateRequest);
            return Ok(new { success = true, message = "Section name updated successfully." });
        }
    }
}
