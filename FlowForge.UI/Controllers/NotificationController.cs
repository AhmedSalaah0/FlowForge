using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return Unauthorized("User not found");
            }

            var notifications = await _notificationService.GetNotifications(user.Id);

            return PartialView("_NotificationPartialView", notifications);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                await _notificationService.MarkAllNotificationsAsRead(user.Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
