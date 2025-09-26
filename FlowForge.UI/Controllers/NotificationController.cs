using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.Hubs;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                var notifications = await notificationService.GetNotifications(user.Id);
                return PartialView("_NotificationPartialView", notifications);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                await notificationService.MarkAllNotificationsAsRead(user.Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> AllNotifications()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                var notifications = await notificationService.GetNotifications(user.Id);
                return View("AllNotifications", notifications);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteNotification(Guid NotificationId)
        {
            if (NotificationId == Guid.Empty)
            {
                throw new KeyNotFoundException("Id is empty");
            }
            bool result = await notificationService.DeleteNotification(NotificationId);
            if (!result)
            {
                TempData["ErrorMessage"] = "Error deleting notification";
            }
            else
            {
                TempData["Message"] = "Notification deleted successfully";
            }
            var link = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(link))
            {
                var uri = new Uri(link);
                var localPath = uri.PathAndQuery;

                if (Url.IsLocalUrl(localPath))
                {
                    return Redirect(localPath);
                }
                return Redirect(link);
            }
            return RedirectToAction("AllNotifications");
        }


        [HttpPost]
        public async Task<IActionResult> ClearNotifications()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            try
            {
                await notificationService.DeleteAllNotifications(user.Id);
                ViewBag.Message = "All notifications cleared.";
                return View("AllNotifications", new List<Core.Domain.Entities.Notification>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error clearing notifications: {ex.Message}";
                return View("AllNotifications", notificationService.GetNotifications(user.Id));
            }
        }
    }
}
