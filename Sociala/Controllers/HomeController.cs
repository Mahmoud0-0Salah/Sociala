using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sociala.Data;
using Sociala.Hubs;
using Sociala.Models;
using Sociala.Services;
using Sociala.ViewModel;
using System.Diagnostics;
using AuthorizationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sociala.Data;
using Sociala.Hubs;
using Sociala.Models;
using Sociala.Services;
using Sociala.ViewModel;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sociala.Migrations;

namespace Sociala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        private readonly IAuthorization authorization;
        private readonly ICheckRelationShip CheckRelationShip;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, AppData data, IAuthorization authorization, ICheckRelationShip CheckRelationShip, IHubContext<NotificationsHub> hubContext)
        {
            _logger = logger;
            _data = data;
            this.authorization = authorization;
            this.CheckRelationShip = CheckRelationShip;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            string id;
            try
            {
                if (!authorization.IsLoggedIn())
                    return RedirectToAction("LogIn", "User");
                id = authorization.GetId();
                if (authorization.IsAdmin(id))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return Redirect("/User/LogOut");
            }

            var friendsId = _data.Friend.Where(f => f.RequestingUserId.Equals(id) || f.RequestedUserId.Equals(id)).Select(f => id.Equals(f.RequestedUserId) ? f.RequestingUserId : f.RequestedUserId).ToList();
            friendsId.Add(id);
            var friends = _data.User.Where(u => friendsId.Contains(u.Id));

            var _userConnections = NotificationsHub.GetUsersConnections();
            ViewBag.friends = friends.Select(u => new { User = u, IsActive = _userConnections.ContainsKey(u.Id) });

            var posts = (_data.Post.Join(friends,
             post => post.UserId,
             friend => friend.Id,
             (post, friend) => new PostInfo
             {
                 Id = post.Id,
                 PostContent = post.content,
                 PostImj = post.Imj,
                 UserPhoto = friend.UrlPhoto,
                 UserName = friend.UesrName,
                 UserId = friend.Id,
                 CreateAt = post.CreateAt,
                 IsHidden = post.IsHidden,
                 IsBanned = friend.IsBanned,
                 Isliked = (_data.Like.Contains(new Like
                 {
                     PostId = post.Id,
                     UserId = id
                 }))
             }))
             .Where(p => !p.IsHidden && !p.IsBanned)
             .ToList();

                    var sharedPosts = _data.SharePost.Include(p => p.Post).Include(p => p.Post.User).Join(friends,
                        post => post.UserId,
                        friend => friend.Id,
                        (post, friend) => new PostInfo
                        {
                            Id = post.Id,
                            OriginalId = post.Post.Id,
                            PostContent = post.Content,
                            OriginalPostContent = post.Post.content,
                            PostImj = post.Post.Imj,
                            UserPhoto = post.User.UrlPhoto,
                            OriginalUserPhoto = post.Post.User.UrlPhoto,
                            UserName = post.User.UesrName,
                            OriginalUserName = post.Post.User.UesrName,
                            UserId = friend.Id,
                            OriginalUserId = post.Post.UserId,
                            CreateAt = post.CreatedAt,
                            IsHidden = (post.IsHidden | post.Post.IsHidden),
                            IsBanned = (friend.IsBanned | post.User.IsBanned),
                            Isliked = (_data.Like.Contains(new Like
                            {
                                PostId = post.Post.Id,
                                UserId = id
                            }))
                        })
                        .Where(p => !p.IsHidden && !p.IsBanned)
                        .ToList();

            posts.AddRange(sharedPosts);




            var RequestsId = _data.Request.Where(r => r.RequestedUserId.Equals(id)).Select(r => r.RequestingUserId);
            ViewBag.Requests = _data.User.Where(u => RequestsId.Contains(u.Id) && !u.IsBanned);
            ViewBag.posts = posts.OrderByDescending(p=>p.CreateAt);
            return View();
        }

        
        public IActionResult SearchForPosts(string type)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string Name = (string)TempData["Name"];
                ViewBag.Name = Name;
                TempData["Name"] = Name;
                string id = authorization.GetId();


                if (type == "Posts")
                    ViewBag.Posts = "active";
                else if (type == "Photos")
                    ViewBag.Photos = "active";
                else 
                    ViewBag.Videos = "active";

                var posts = (_data.Post.Join(_data.User,
                 post => post.UserId,
                 friend => friend.Id,
                 (post, friend) => new PostInfo
                 {
                     Id = post.Id,
                     PostContent = post.content,
                     PostImj = post.Imj,
                     UserPhoto = friend.UrlPhoto,
                     UserName = friend.UesrName,
                     UserId = friend.Id,
                     CreateAt = post.CreateAt,
                     IsHidden = post.IsHidden,
                     IsBanned = friend.IsBanned,
                     Isliked = (_data.Like.Contains(new Like
                     {
                         PostId = post.Id,
                         UserId = id
                     }))
                 }))
                 .Where(p => !p.IsHidden && !p.IsBanned && p.PostContent.Contains(Name))
                 .ToList();

         
                var sharedPosts = _data.SharePost.Include(p => p.Post).Include(p => p.Post.User).Join(_data.User,
                    post => post.UserId,
                    friend => friend.Id,
                    (post, friend) => new PostInfo
                    {
                        Id = post.Id,
                        OriginalId = post.Post.Id,
                        PostContent = post.Content,
                        OriginalPostContent = post.Post.content,
                        PostImj = post.Post.Imj,
                        UserPhoto = post.User.UrlPhoto,
                        OriginalUserPhoto = post.Post.User.UrlPhoto,
                        UserName = post.User.UesrName,
                        OriginalUserName = post.Post.User.UesrName,
                        UserId = friend.Id,
                        OriginalUserId = post.Post.UserId,
                        CreateAt = post.CreatedAt,
                        IsHidden = (post.IsHidden | post.Post.IsHidden),
                        IsBanned = (friend.IsBanned | post.User.IsBanned),
                        Isliked = (_data.Like.Contains(new Like
                        {
                            PostId = post.Post.Id,
                            UserId = id
                        }))
                    })
                    .Where(p => !p.IsHidden && !p.IsBanned && p.PostContent.Contains(Name))
                    .ToList();

                posts.AddRange(sharedPosts);

                if (type == "Photos")
                    posts = posts.Where(p => p.PostImj!=null && p.PostImj[p.PostImj.Length - 1] != '4').ToList();
                else if (type == "Videos" )
                    posts = posts.Where(p => p.PostImj != null && p.PostImj[p.PostImj.Length - 1] == '4').ToList();

                return View(posts);
        }

        [HttpPost]
        public IActionResult Search(IFormCollection req)
        {
                return Redirect($"/Home/Search/?name={req["Name"]}");
        }

        [HttpGet]
        public IActionResult Search(string Name)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            TempData["Name"] = Name;

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
                                    UrlPhoto = User.UrlPhoto,
                                    status = User.IsBanned

                                })).Where((result => result.Role != "Admin" && !result.status && result.IsActive && result.UesrName.Contains(Name))).ToList();
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




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}