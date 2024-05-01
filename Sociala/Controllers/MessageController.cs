using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sociala.Data;
using Sociala.Hubs;
using Sociala.Models;
using System.Text.Json;

namespace Sociala.Controllers
{
    public class MessageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        private readonly IAuthorization authorization;
        private readonly IHubContext<NotificationsHub> _hubContext;
        public MessageController(ILogger<HomeController> logger, AppData data, IAuthorization authorization, IHubContext<NotificationsHub> hubContext)
        {
            _logger = logger;
            _data = data;
            this.authorization = authorization;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> CreateMessage(string id, string content)
        {
            string UserId = authorization.GetId();
            Message message = new Message();
            message.SenderId = UserId;
            message.Content = content;
            message.ResverId = id;
            _data.Message.Add(message);
            _data.SaveChanges();


            string jsonData = JsonSerializer.Serialize(message);
            var _userConnections = NotificationsHub.GetUsersConnections();
            if (!string.IsNullOrEmpty(id) && _userConnections.TryGetValue(id, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("Message", jsonData);
                }
            }



            var Message = _data.Message.Where(m=>(m.SenderId.Equals(UserId) && m.ResverId.Equals(id)|| m.SenderId.Equals(id) && m.ResverId.Equals(UserId)));
            ViewBag.UserId=UserId;
            ViewBag.Sendder = _data.User.Where(u=>u.Id.Equals(id)).SingleOrDefault();
            return PartialView("ShowMessage", Message);
        }

        public IActionResult ShowMessage(string id)
        {
            string UserId = authorization.GetId();
            var Message = _data.Message.Where(m => (m.SenderId.Equals(UserId) && m.ResverId.Equals(id) || m.SenderId.Equals(id) && m.ResverId.Equals(UserId)));
            ViewBag.UserId = UserId;
            ViewBag.Sendder = _data.User.Where(u => u.Id.Equals(id)).SingleOrDefault();

            return PartialView(Message);
        }
    }
}
