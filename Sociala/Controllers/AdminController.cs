using AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Models;

namespace Sociala.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppData _data;
        private readonly IAuthorization authorization;
        public AdminController(ILogger<HomeController> logger, AppData data, IAuthorization authorization)
        {
            _data = data;
            this.authorization = authorization;
        }

        public IActionResult Index()
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string id = authorization.GetId();
            if (!authorization.IsAdmin(id))
            {
                return RedirectToAction("Index", "Home");
            }

            var reports = _data.Report.Where(r => r.Status == "Pending").Include(r => r.User);
            ViewBag.NumOfReportedUsers = _data.Report.Where(r => r.Status.Equals("Approved")).Include(r => r.Post).Include(r => r.Post.User).Select(r => r.Post.User).Where(u => !u.IsBanned).Distinct().Count();
            return View(reports);
        }

        [HttpGet]
        public IActionResult Report(int Id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (!authorization.IsAdmin(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var report = _data.Report.Where(r => r.Id == Id).Include(r => r.User)
                                                            .Include(r => r.Post).Include(r => r.Post.User)
                                                            .SingleOrDefault();
            if (report == null || report.Status!= "Pending")
            {

                return RedirectToAction("Index", "Admin");
            }

            return View(report);
        }
        public IActionResult ReportedUsersInfo()
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (!authorization.IsAdmin(userId))
            {
                return RedirectToAction("Index", "Home");
            }
            var users = _data.Report.Where(r=>r.Status.Equals("Approved")).Include(r => r.Post).Include(r => r.Post.User).Select(r=>r.Post.User).Where(u=>!u.IsBanned).Distinct().OrderByDescending(u => u.NumberOfApprovedReports);
            ViewBag.ReportNum= _data.Report.Where(r => r.Status == "Pending").Count();
            return View(users);
        }
        [HttpGet]
        public IActionResult ApproveReport(int Id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (!authorization.IsAdmin(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var report = _data.Report.Where(r => r.Id == Id).Include(r => r.Post).Include(r=>r.Post.User).SingleOrDefault();
            if (report == null || report.Status != "Pending")
            {
                return RedirectToAction("Index", "Admin");
            }

            report.Status = "Approved";
            report.Post.IsHidden = true;
            report.Post.User.NumberOfApprovedReports +=1;
            _data.SaveChanges();

            return View();
        }
        
        public IActionResult RejectReport(int Id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (!authorization.IsAdmin(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var report = _data.Report.Where(r => r.Id == Id).SingleOrDefault();
            if (report == null || report.Status != "Pending")
            {
                return RedirectToAction("Index", "Admin");
            }

            report.Status = "Rejected";
            _data.SaveChanges();

            return View();
        }
        public IActionResult Block(string id)
        {
            if (!authorization.IsLoggedIn())
            {
                return RedirectToAction("LogIn", "User");
            }
            string userId = authorization.GetId();
            if (!authorization.IsAdmin(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = _data.User.Where(u=>u.Id.Equals(id)).SingleOrDefault();
            user.IsBanned = true;
            _data.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
