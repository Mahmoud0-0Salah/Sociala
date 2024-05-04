using System.Net;
using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Services;

namespace Sociala.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AppData _data;
        private readonly IAuthorization _authorization;
        private readonly INotification _notificationService;
        public NotificationsController(AppData data, IAuthorization authorization, INotification notificationService)
        {
            _data = data;
            _authorization = authorization;
            _notificationService = notificationService;
        }
        public IActionResult NotificationList()
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string id = _authorization.GetId();
            var notifications = _data.Notification.Where(n => n.UserId == id).Include(n => n.Actor)
                                                   .OrderByDescending(n => n.CreatedAt);
            return View(notifications);
        }

        public async Task<IActionResult> MarkAsRead(int Id)
        {

            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();
            var notification = _data.Notification.SingleOrDefault(n => n.Id == Id);
            if (notification == null || notification.UserId != userId)
            {
                return RedirectToAction("index", "Home");
            }

            await _notificationService.MarkAsRead(notification);

            return Json(new { success = true});

        }
    }
}
