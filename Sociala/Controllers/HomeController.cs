using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;
using Sociala.ViewModel;
using System.Diagnostics;
using System.Linq;

namespace Sociala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppData _data;
        public HomeController(ILogger<HomeController> logger,AppData data)
        {
            _logger = logger;
            _data = data;
        }

        public IActionResult Index()
        {
            string id="a";
            var friendsId = _data.Friend.Where(f => f.RequestingUserId.Equals(id) || f.RequestedUserId.Equals(id)).Select(f=>f.RequestedUserId);
            var friends = _data.User.Where(u=>friendsId.Contains(u.Id));
            ViewBag.posts = _data.Post.Join(friends,
                                post => post.UserId,
                                friend => friend.Id,
                                (post, friend) => new PostInfo
                                {
                                    PostContent=post.content,
                                    PostImj = post.Imj,
                                    UserPhoto = friend.UrlPhoto,
                                    UserName = friend.UesrName
                                });
            var RequestsId = _data.Request.Where(r => r.RequestingUserId.Equals(id)).Select(r=>r.RequestedUserId);
            ViewBag.Requests = _data.User.Where(u => RequestsId.Contains(u.Id));

            return View();
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