using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Models;

namespace Sociala.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppData _data;
        private readonly IAuthorization authorization;
        public PostsController(ILogger<HomeController> logger, AppData data, IAuthorization authorization)
        {
            _data = data;
            this.authorization = authorization;
        }



        [HttpGet]
        public IActionResult Report(int Id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();

            var post = _data.Post.Where(p => p.Id == Id).Include(p => p.User).SingleOrDefault();
            if (post == null) {

                return RedirectToAction("Index", "Home");
            }
            if (! CanReport(Id, userId))
            {
                return View("CanNotReport");
            }

            return View(post);
        }
        
        [HttpPost]
        public IActionResult Report(int Id, IFormCollection req)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();

            var post = _data.Post.FirstOrDefault(p => p.Id == Id);
            if (post == null) {

                return RedirectToAction("Index", "Home");
            }
            if (!CanReport(Id, userId))
            {
                return View("CanNotReport");
            }

            var content = req["content"];
            var report = new Report{  UserId = userId, PostId=Id, Content=content };

            _data.Add(report);  
            _data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public bool CanReport(int postId, string userId)
        {
            var post = _data.Post.FirstOrDefault(p => p.Id == postId);
            if (post.UserId == userId)
            {
                return false;
            }
            var report = _data.Report.FirstOrDefault(r => r.PostId == postId && r.UserId == userId);
            if (report == null)
            {
                return true;
            }
            return false;
        }
    }
}
