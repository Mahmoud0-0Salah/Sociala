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

        public IActionResult DeletePost(int Id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");

            var Post =_data.Post.Where(p=>p.Id == Id).SingleOrDefault();
            Post.IsHidden = true;
            _data.SaveChanges();
            return Redirect("/user/profile");
        }
        public IActionResult SharePost( int Id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");
            Post Result = _data.Post.Where(p => p.Id == Id).SingleOrDefault();
            Post post= new Post();
            post.UserId = authorization.GetId();
            post.CreateAt = DateTime.Now;
            post.Imj = Result.Imj;
            post.content = Result.content;
            post.IsHidden = Result.IsHidden;
            _data.Post.Add(post);
            _data.SaveChanges();

            return Redirect("/Home/Index");
        }

        public IActionResult Like(int Id,string Place)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");
            Like like=new Like();
            like.UserId = authorization.GetId();
            like.PostId = Id;
            _data.Like.Add(like);
            _data.SaveChanges();
            if (Place == "Index")
                return Json(new { success = true, redirectTo = "/Home/Index" });
            return Json(new { success = true, redirectTo = $"/User/Profile/{Place}" });

        }

        public IActionResult DeleteLike(int Id, string Place)
        {

            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");

            var target= _data.Like.Where(l => l.PostId == Id && l.UserId == authorization.GetId()).SingleOrDefault();
            _data.Like.Remove(target);
            _data.SaveChanges();
            if (Place == "Index")
                return Json(new { success = true, redirectTo = "/Home/Index" });
            return Json(new { success = true, redirectTo = $"/User/Profile/{Place}" });

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
