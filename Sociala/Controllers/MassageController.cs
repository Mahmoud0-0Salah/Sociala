using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;

namespace Sociala.Controllers
{
    public class MassageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        private readonly IAuthorization authorization;

        public MassageController(ILogger<HomeController> logger, AppData data, IAuthorization authorization)
        {
            _logger = logger;
            _data = data;
            this.authorization = authorization;
        }
        public IActionResult CreateMassage(string id, string content)
        {
            string UserId = authorization.GetId();
            Massage massage = new Massage();
            massage.SenderId = UserId;
            massage.Content = content;
            massage.ResverId = id;
            _data.Massage.Add(massage);
            _data.SaveChanges();
            var Massage = _data.Massage.Where(m=>(m.SenderId.Equals(UserId) && m.ResverId.Equals(id)|| m.SenderId.Equals(id) && m.ResverId.Equals(UserId)));
            ViewBag.UserId=UserId;
            ViewBag.Sendder = _data.User.Where(u=>u.Id.Equals(id)).SingleOrDefault();
            return PartialView("ShowMassage", Massage);
        }

        public IActionResult ShowMassage(string id)
        {
            string UserId = authorization.GetId();
            var Massage = _data.Massage.Where(m => (m.SenderId.Equals(UserId) && m.ResverId.Equals(id) || m.SenderId.Equals(id) && m.ResverId.Equals(UserId)));
            ViewBag.UserId = UserId;
            ViewBag.Sendder = _data.User.Where(u => u.Id.Equals(id)).SingleOrDefault();

            return PartialView(Massage);
        }
    }
}
