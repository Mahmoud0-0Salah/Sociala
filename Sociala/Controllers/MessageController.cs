using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;

namespace Sociala.Controllers
{
    public class MessageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        private readonly IAuthorization authorization;

        public MessageController(ILogger<HomeController> logger, AppData data, IAuthorization authorization)
        {
            _logger = logger;
            _data = data;
            this.authorization = authorization;
        }
        public IActionResult CreateMessage(string id, string content)
        {
            string UserId = authorization.GetId();
            Message message = new Message();
            message.SenderId = UserId;
            message.Content = content;
            message.ResverId = id;
            _data.Message.Add(message);
            _data.SaveChanges();
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
