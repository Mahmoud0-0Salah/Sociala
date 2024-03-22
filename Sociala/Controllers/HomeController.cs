using AuthorizationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;
using Sociala.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;

namespace Sociala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        private readonly IAuthorization authorization;
        public HomeController(ILogger<HomeController> logger,AppData data, IAuthorization authorization)
        {
            _logger = logger;
            _data = data;
            this.authorization = authorization;
        }

        public IActionResult Index()
        {
            if (!authorization.IsLoggedIn())
                return RedirectToAction("LogIn", "User");
            string id=authorization.GetId();
            var friendsId = _data.Friend.Where(f => f.RequestingUserId.Equals(id) || f.RequestedUserId.Equals(id)).Select(f=> id.Equals(f.RequestedUserId) ? f.RequestingUserId : f.RequestedUserId);
            var friends = _data.User.Where(u=>friendsId.Contains(u.Id));
            ViewBag.posts = (_data.Post.Join(friends,
                                post => post.UserId,
                                friend => friend.Id,
                                (post, friend) => new PostInfo
                                {
                                    PostContent=post.content,
                                    PostImj = post.Imj,
                                    UserPhoto = friend.UrlPhoto,
                                    UserName = friend.UesrName,
                                    CreateAt = post.CreateAt
                                })).OrderBy(p=>p.CreateAt);
            var RequestsId = _data.Request.Where(r => r.RequestingUserId.Equals(id)).Select(r=>r.RequestedUserId);
            ViewBag.Requests = _data.User.Where(u => RequestsId.Contains(u.Id));

            return View();
        }
        [HttpPost]
        public IActionResult CreatePost(string content)
        {
            if (!authorization.IsLoggedIn())
                return RedirectToAction("LogIn", "User");
            var post = new Post
            {
                content = content,
                UserId = authorization.GetId(),
            CreateAt = DateTime.Now,
            };
            var file = HttpContext.Request.Form.Files;

            if (file.Count() > 0)
            {
                if (!Path.GetExtension(file[0].FileName).Equals(".jpg") && !Path.GetExtension(file[0].FileName).Equals(".png") && !Path.GetExtension(file[0].FileName).Equals(".jpeg")
                    && !Path.GetExtension(file[0].FileName).Equals(".mp4"))
                {
                    TempData["PhotoMessage"] = "Upload photo with Extension mp4,JPG,PNG or JPEG";
                    return RedirectToAction("Index");
                }
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                var filePath = Path.Combine("wwwroot", "imj", imageName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file[0].CopyTo(fileStream); // Save in the Images folder
                }

                post.Imj = $"/imj/{imageName}";
            }

            _data.Post.Add(post);
            _data.SaveChanges();
           

            return RedirectToAction("Index"); 
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}