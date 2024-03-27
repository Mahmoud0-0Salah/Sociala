using AuthorizationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
           
            string id=authorization.GetId();

            var RequestsId = _data.Request.Where(r => r.RequestingUserId.Equals(id)).Select(r=>r.RequestedUserId);
            ViewBag.Requests = _data.User.Where(u => RequestsId.Contains(u.Id));

            return View();
        }
        [HttpPost]

        public IActionResult CreatePost(string? content)
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
                    file[0].CopyTo(fileStream); 
                }

                post.Imj = $"/imj/{imageName}";
            }
            else if (file.Count() <= 0 && content == null)
            {
                TempData["PhotoMessage"] = "The Post is empty please Enter  anything";
                return RedirectToAction("Index");
            }
            

            _data.Post.Add(post);
            _data.SaveChanges();
           

            return RedirectToAction("Index"); 
        }
        public IActionResult AddFriend(String Id)//how to take Id from View
        {
            var request = new Request();
            request.RequestingUserId = authorization.GetId();
            request.RequestedUserId =Id;
            _data.Request.Add(request);
            _data.SaveChanges();

            return View("Search");

        }
        public IActionResult ConfirmRequest(string Id)
        {
            Friend friend =new Friend();
            friend.RequestedUserId= authorization.GetId();
            friend.RequestingUserId = Id;
            _data.Friend.Add(friend);
            var DeleteResult = _data.Request.Where(p => p.RequestedUserId == authorization.GetId() && p.RequestingUserId == Id).Select(f => f.Id);
            var FinalResult = _data.Request.Find(DeleteResult.First());
           _data.Request.Remove(FinalResult);
            _data.SaveChanges();
            
            return View("Index");
        }

        public IActionResult DeleteRequest(string Id)
        {
            

            var DeleteResult = _data.Request.Where(p => p.RequestedUserId == authorization.GetId() && p.RequestingUserId == Id).Select(f=>f.Id);
            var FinalResult = _data.Request.Find(DeleteResult.First());
            _data.Request.Remove(FinalResult);
            _data.SaveChanges();
            return View("Index");

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