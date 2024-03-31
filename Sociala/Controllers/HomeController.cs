using AuthorizationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;
using Sociala.Services;
using Sociala.ViewModel;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace Sociala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        private readonly IAuthorization authorization;
        private readonly ICheckRelationShip CheckRelationShip;
        public HomeController(ILogger<HomeController> logger,AppData data, IAuthorization authorization, ICheckRelationShip CheckRelationShip)
        {
            _logger = logger;
            _data = data;
            this.authorization = authorization;
            this.CheckRelationShip = CheckRelationShip;

        }

        public IActionResult Index()
        {
            if (!authorization.IsLoggedIn())
                return RedirectToAction("LogIn", "User");
            string id=authorization.GetId();
            if (authorization.IsAdmin(id))
            {
                return RedirectToAction("Index", "Admin");
            }

            var friendsId = _data.Friend.Where(f => f.RequestingUserId.Equals(id) || f.RequestedUserId.Equals(id)).Select(f=> id.Equals(f.RequestedUserId) ? f.RequestingUserId : f.RequestedUserId).ToList();
            friendsId.Add(id);
            var friends = _data.User.Where(u=>friendsId.Contains(u.Id));
            ViewBag.posts = (_data.Post.Join(friends,
                                post => post.UserId,
                                friend => friend.Id,
                                (post, friend) =>
                                
                                new PostInfo
                                ()
                                {
                                    Id = post.Id,
                                    PostContent = post.content,
                                    PostImj = post.Imj,
                                    UserPhoto = friend.UrlPhoto,
                                    UserName = friend.UesrName,
                                    CreateAt = post.CreateAt,
                                    IsHidden = post.IsHidden,
                                    
                                    Isliked = ((!(_data.Like.Contains(new Like
                                    {
                                        PostId = post.Id,
                                        UserId = id
                                    }))) ? false
                                    : true),

                                }
                                )).Where(p => !p.IsHidden).OrderByDescending(p => p.CreateAt);
            var RequestsId = _data.Request.Where(r => r.RequestedUserId.Equals(id)).Select(r=>r.RequestingUserId);
            ViewBag.Requests = _data.User.Where(u => RequestsId.Contains(u.Id));

            return View();
        }
        [HttpGet]
         public IActionResult Search()
        {
            string Name =Convert.ToString( TempData["Name"]);
            ViewBag.Search = (_data.User.Join(_data.Role,
                                User => User.RoleId,
                                Role => Role.Id,
                                
                                (User, Role) => new 
                                {
                                    Role=Role.Name,
                                    Id = User.Id,
                                    UesrName = User.UesrName,
                                    Email = User.Email,
                                    PhoneNumber = User.PhoneNumber,
                                    bio = User.bio,
                                    IsActive = User.IsActive,
                                    UrlPhoto=User.UrlPhoto

                                })).Where((result => result.Role != "Admin" && result.IsActive && result.UesrName.Contains( Name))).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Search(string Name)
        {
            if (Name.Length == 0) Name = Convert.ToString(TempData["Name"]);
            ViewBag.Search = (_data.User.Join(_data.Role,
                                User => User.RoleId,
                                Role => Role.Id,

                                (User, Role) => new
                                {
                                    Role = Role.Name,
                                    Id = User.Id,
                                    UesrName = User.UesrName,
                                    Email = User.Email,
                                    PhoneNumber = User.PhoneNumber,
                                    bio = User.bio,
                                    IsActive = User.IsActive,
                                    UrlPhoto = User.UrlPhoto

                                })).Where((result => result.Role != "Admin" && result.IsActive && result.UesrName.Contains(Name))).ToList();
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
                    file[0].CopyTo(fileStream); // Save in the Images folder
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