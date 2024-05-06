using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Models;
using Sociala.Services;

namespace Sociala.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppData _data;
        private readonly IAuthorization _authorization;
        private readonly INotification _notificationSerivce;
        public PostsController(ILogger<HomeController> logger, AppData data,
                               IAuthorization authorization, INotification notificationSerivce)
        {
            _data = data;
            _authorization = authorization;
            _notificationSerivce = notificationSerivce;
        }

        public IActionResult DeletePost(int Id, bool IsShared = false)
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();
            if (_authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");

            if (!IsShared)
            {
                var Post = _data.Post.Where(p => p.Id == Id).SingleOrDefault();
                Post.IsHidden = true;
            }
            else
            {
                var Post = _data.SharePost.Where(p => p.Id == Id).SingleOrDefault();
                Post.IsHidden = true;
            }
            _data.SaveChanges();
            return Redirect("/user/profile");
        }
        public IActionResult EditPost(int Id)
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }

            var post = _data.Post.FirstOrDefault(p => p.Id == Id);

            if (post == null)
            {
                return NotFound();
            }

            string userId = _authorization.GetId();
            if (post.UserId != userId)
            {
                return View("ErrorPage");
            }

            return View(post);
        }
        [HttpPost]
        public IActionResult EditPost(Post updatedPost, bool deleteImage)
        {
            var post = _data.Post.FirstOrDefault(p => p.Id == updatedPost.Id);

            if (post == null)
            {
                return NotFound();
            }

            string userId = _authorization.GetId();
            if (post.UserId != userId)
            {
                return View("ErrorPage");
            }

            post.content = updatedPost.content;

            if (deleteImage)
            {
                post.Imj = null;
            }
            else
            {
                var file = HttpContext.Request.Form.Files;

                if (file.Count() > 0)
                {
                    if (!Path.GetExtension(file[0].FileName).Equals(".jpg") && !Path.GetExtension(file[0].FileName).Equals(".png") && !Path.GetExtension(file[0].FileName).Equals(".jpeg")
                            && !Path.GetExtension(file[0].FileName).Equals(".mp4"))
                    {
                        ViewBag.PhotoMessage = "Upload photo with Extension JPG,PNG,JPEG, or MP4";

                        return View(post);
                    }
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                    var filePath = Path.Combine("wwwroot", "imj", imageName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file[0].CopyTo(fileStream);
                    }

                    post.Imj = $"/imj/{imageName}";
                }
            }
            if (post.Imj == null && post.content == null)
            {
                TempData["PhotoMessage"] = "The Post is empty please Enter  anything";
                return RedirectToAction("EditPost");
            }
            _data.SaveChanges();

            return RedirectToAction("Profile", "User");


        }

        [HttpGet]
        public IActionResult SharePost(int Id, string? UserId)
        {
            var post = _data.Post.Where(p => p.Id == Id).Include(p => p.User).SingleOrDefault();
            TempData["FromProfile"] = UserId;
            if (post == null)
            {
                if (UserId == null)
                    return Redirect("/Home/Index");
                else
                {
                    return Redirect($"/user/Profile/{UserId}");
                }
            }
            return View(post);

        }
        [HttpPost]
        public IActionResult SharePost(int Id, IFormCollection req)
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();
            if (_authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");
            SharePost post = new SharePost();
            post.UserId = userId;
            post.PostId = Id;
            post.Content = req["content"];
            _data.SharePost.Add(post);
            _data.SaveChanges();

            if (TempData["FromProfile"] == null)
                return Redirect("/Home/Index");
            else
            {
                return Redirect($"/user/Profile/{TempData["FromProfile"]}");
            }
        }

        public async Task<IActionResult> Like(int Id, string Place)
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();
            if (_authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");
            Like like = new Like();
            like.UserId = _authorization.GetId();
            like.PostId = Id;
            _data.Like.Add(like);
            _data.SaveChanges();
            await _notificationSerivce.SendLikeNotification(userId, Id);

            if (Place == "Index")
                return Json(new { success = true, redirectTo = "/Home/Index" });
            return Json(new { success = true, redirectTo = $"/User/Profile/{Place}" });

        }

        public IActionResult DeleteLike(int Id, string Place)
        {

            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();
            if (_authorization.IsAdmin(userId))
                return RedirectToAction("index", "Home");

            var target = _data.Like.Where(l => l.PostId == Id && l.UserId == _authorization.GetId()).SingleOrDefault();
            _data.Like.Remove(target);
            _data.SaveChanges();
            if (Place == "Index")
                return Json(new { success = true, redirectTo = "/Home/Index" });
            return Json(new { success = true, redirectTo = $"/User/Profile/{Place}" });

        }

        public IActionResult CreateComment(int id, string content)
        {
            try
            {
                Comment comment = new Comment();
                comment.PostId = id;
                comment.UserId = _authorization.GetId();
                comment.Content = content;
                comment.CreatedAt = DateTime.Now;
               
                _data.Comment.Add(comment);
                _data.SaveChanges();
                var res = _data.Comment.Where(p => p.PostId == id).Include(p => p.User);
                return PartialView("ShowComment", res);
            }
            catch
            {
                return View("ErrorPage");
            }
        }
        public IActionResult ShowComment(int Id)
        {
          
            var comment = _data.Comment.Where(p => p.PostId == Id).Include(p => p.User);
            TempData["PostId"]= Id;
            return PartialView(comment);
        }

        [HttpGet]
        public IActionResult Report(int Id)
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();

            var post = _data.Post.Where(p => p.Id == Id).Include(p => p.User).SingleOrDefault();
            if (post == null)
            {

                return RedirectToAction("Index", "Home");
            }
            if (!CanReport(Id, userId))
            {
                return View("CanNotReport");
            }

            return View(post);
        }

        [HttpPost]
        public IActionResult Report(int Id, IFormCollection req)
        {
            if (!_authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = _authorization.GetId();

            var post = _data.Post.FirstOrDefault(p => p.Id == Id);
            if (post == null)
            {

                return RedirectToAction("Index", "Home");
            }
            if (!CanReport(Id, userId))
            {
                return View("CanNotReport");
            }

            var content = req["content"];
            var report = new Report { UserId = userId, PostId = Id, Content = content };

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
