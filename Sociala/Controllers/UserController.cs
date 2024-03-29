using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Sociala.Data;
using Sociala.Models;
using Sociala.ViewModel;
using EncryptServices;
using EmailSendertServices;
using System;
using AuthorizationService;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Sociala.Controllers
{
    public class UserController : Controller
    {
        private readonly AppData _context;

        private readonly AppData appData;
        private readonly IEmailSender emailSender;
        private readonly IEncrypt encryptclass;
        private readonly IAuthorization authorization;
        private readonly AppData _data;
        public UserController(AppData appData, IEncrypt encryptClass, IEmailSender emailSender, IAuthorization authorization, AppData data)
        {
            this.appData = appData;
            this.encryptclass = encryptClass;
            this.emailSender = emailSender;
            this.authorization = authorization;
            _data = data;


        }
        public IActionResult Profile()
        {
            string userId = authorization.GetId();

            var user = appData.User.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (!authorization.IsLoggedIn())
                return RedirectToAction("LogIn", "User");

            string id = authorization.GetId();
            ViewBag.posts = _data.Post.Where(post => post.UserId == id)
                                      .Join(_data.User,
                                            post => post.UserId,
                                            user => user.Id,
                                            (post, user) => new PostInfo
                                            {
                                                PostContent = post.content,
                                                PostImj = post.Imj,
                                                UserPhoto = user.UrlPhoto,
                                                UserName = user.UesrName,
                                                CreateAt = post.CreateAt
                                            })
                                      .OrderBy(p => p.CreateAt);

            var RequestsId = _data.Request.Where(r => r.RequestingUserId.Equals(id)).Select(r => r.RequestedUserId);
            ViewBag.Requests = _data.User.Where(u => RequestsId.Contains(u.Id));

            return View(user);
        }
        public IActionResult EditProfile()
        {
            string userId = authorization.GetId();
            var user = appData.User.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser, IFormFile photo)
        {
            string userId = authorization.GetId();
            var user = appData.User.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            user.UesrName = updatedUser.UesrName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.bio = updatedUser.bio;
            var file = HttpContext.Request.Form.Files;

            if (file.Count() > 0)
            {
                if (!Path.GetExtension(file[0].FileName).Equals(".jpg") && !Path.GetExtension(file[0].FileName).Equals(".png") && !Path.GetExtension(file[0].FileName).Equals(".jpeg"))
                {
                    ViewBag.PhotoMessage = "Upload photo with Extension JPG,PNG or JPEG";
                    return View();
                }
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                var filePath = Path.Combine("wwwroot", "images", imageName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }

                user.UrlPhoto = $"/images/{imageName}";
            }
            else
                user.UrlPhoto = user.UrlPhoto;

            appData.SaveChanges();

            return RedirectToAction("Profile");
        }
        public IActionResult AddFriend(String Id, String Place)
        {
           
            var request = new Request();
            request.RequestingUserId = authorization.GetId();
            request.RequestedUserId = Id;
            appData.Request.Add(request);
            appData.SaveChanges();
            Console.WriteLine(Place);
            if (Place == "Search") { 

                var result=appData.User.Where(u => u.Id == Id).FirstOrDefault();
                TempData["Name"] =result.UesrName;
                return Redirect("/Home/Search");
            }
            else return RedirectToAction("Profile");




        }
        public IActionResult DeleteRequest(string Id)
        {


            var DeleteResult = appData.Request.Where(p => p.RequestedUserId == authorization.GetId() && p.RequestingUserId == Id).Select(f => f.Id);
            var FinalResult = appData.Request.Find(DeleteResult.First());
            appData.Request.Remove(FinalResult);
            appData.SaveChanges();
            return Redirect("/Home/Index");

        }
        public IActionResult ConfirmRequest(string Id)
        {
            Friend friend = new Friend();
            friend.RequestedUserId = authorization.GetId();
            friend.RequestingUserId = Id;
            appData.Friend.Add(friend);
            var DeleteResult = appData.Request.Where(p => p.RequestedUserId == authorization.GetId() && p.RequestingUserId == Id).Select(f => f.Id);
            var FinalResult = appData.Request.Find(DeleteResult.First());
            appData.Request.Remove(FinalResult);
            appData.SaveChanges();
            return Redirect("/Home/Index");
        }
        public IActionResult ShowRequest()
        {
            string id = authorization.GetId();
            var RequestsId = appData.Request.Where(r => r.RequestingUserId.Equals(id)).Select(r => r.RequestedUserId);
            ViewBag.Requests = appData.User.Where(u => RequestsId.Contains(u.Id));

            return View();
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                return false;
            }


            return true;
        }
        /**************************************************************************************/

        private string Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        /**************************************************************************************/


        public IActionResult Login()
        {
            if (authorization.IsLoggedIn())
                return RedirectToAction("index", "Home");
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginInfo loginInfo)
        {
            var user = appData.User.SingleOrDefault(u => u.Email.Equals(loginInfo.Email));
            string slot = Guid.NewGuid().ToString();
            if (user == null)
            {
                ViewBag.NonFoundEmailMessage = "Wrong Email";
                return View();
            }
            if (!Hash(loginInfo.Password).Equals(user.Password))
            {
                ViewBag.WrongPassword = "Wrong password";
                return View();
            }
            if (!user.IsActive)
            {
                Response.Cookies.Append("ActiveKey", encryptclass.Encrypt(user.ActiveKey, slot));
                Response.Cookies.Append("TempId", encryptclass.Encrypt(user.Id, slot));
                Response.Cookies.Append("TempSlot", slot);
                return RedirectToAction("ConfirmEmail", "User");
            }
            if (loginInfo.RememberMe)
            {
                CookieOptions cookie = new CookieOptions();
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("id", encryptclass.Encrypt(user.Id, slot), cookie);
                Response.Cookies.Append("slot", slot, cookie);
            }
            else
            {
                Response.Cookies.Append("id", encryptclass.Encrypt(user.Id, slot));
                Response.Cookies.Append("slot", slot);
            }
            return RedirectToAction("index", "Home");
        }

        /**************************************************************************************/

        [HttpPost]
        public IActionResult LogOut()
        {
            CookieOptions cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Append("id", "1", cookie);
            Response.Cookies.Append("slot", "1", cookie);
            return RedirectToAction("index", "Home");
        }
        /**************************************************************************************/

        public IActionResult SignUp()
        {
            if (Request.Cookies["id"] != null)
                return RedirectToAction("index", "Home");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            bool emailExists = appData.User.Any(u => u.Email == user.Email);

            if (emailExists)
            {
                ViewBag.EmailMessage = "Email already exists. Please use a different email.";
                return View();
            }
            if (user.PhoneNumber.Length != 11 || !(user.PhoneNumber.StartsWith("010") ||
                                             user.PhoneNumber.StartsWith("011") ||
                                             user.PhoneNumber.StartsWith("012") ||
                                             user.PhoneNumber.StartsWith("015")))
            {
                ViewBag.PhoneNumberMessage = "Enter valid egyptian phonenumber";
                return View();
            }
            if (!IsPasswordValid(user.Password))
            {
                ViewBag.PasswordMessage = "Password must be 8 characters long , at least contain one uppercase , at least one lowercase character and at least one symbol.";
                return View();
            }
            var file = HttpContext.Request.Form.Files;

            if (file.Count() > 0)
            {
                if (!Path.GetExtension(file[0].FileName).Equals(".jpg") && !Path.GetExtension(file[0].FileName).Equals(".png") && !Path.GetExtension(file[0].FileName).Equals(".jpeg"))
                {
                    ViewBag.PhotoMessage = "Upload photo with Extension JPG,PNG or JPEG";
                    return View();
                }
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                var filePath = Path.Combine("wwwroot", "imj", imageName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file[0].CopyTo(fileStream); // Save in the Images folder
                }

                user.UrlPhoto = $"/imj/{imageName}";
            }
            else
                user.UrlPhoto = $"/imj/default.jpg";
            user.Id = Guid.NewGuid().ToString();
            user.Password = Hash(user.Password);
            user.IsActive = false;
            user.CreateAt= DateTime.Now;
            user.RoleId = 0;
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                user.ActiveKey += random.Next(0, 10); 
            }
            try
            {
                await emailSender.SendEmailAsync(user.Email, "Confirm email", @"
                <html>
                <head>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            margin: 0;
                            padding: 0;
                        }

                        .container {
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 5px;
                        }

                        .header {
                            background-color: #f5f5f5;
                            padding: 10px 0;
                            text-align: center;
                            border-bottom: 1px solid #ddd;
                        }

                        .body-content {
                            padding: 20px 0;
                        }

                        .verification-code {
                            font-size: 18px;
                            font-weight: bold;
                            color: #007bff;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Welcome to Sociala</h2>
                        </div>
                        <div class='body-content'>
                            <p>Dear " + user.UesrName + @",</p>
                            <p>Thank you for choosing Sociala, your gateway to a vibrant online community.</p>
                            <p>To verify your account, please use the following verification code:</p>
                            <p class='verification-code'>" + user.ActiveKey + @"</p>
                            <p>Thank you.</p>
                        </div>
                    </div>
                </body>
                </html>
                ");

                appData.Add(user);
                await appData.SaveChangesAsync();
                string slot = Guid.NewGuid().ToString();
                Response.Cookies.Append("ActiveKey", encryptclass.Encrypt(user.ActiveKey, slot));
                Response.Cookies.Append("TempId", encryptclass.Encrypt(user.Id, slot));
                Response.Cookies.Append("TempSlot", slot);
            }
            catch
            {
                return View("ErrorPage");
            }
            return RedirectToAction("ConfirmEmail", "User");
        }
        /**************************************************************************************/

        public IActionResult ConfirmEmail()
        {
            if (Request.Cookies["TempId"] == null || Request.Cookies["id"] != null)
                return RedirectToAction("index", "Home");
            return View();
        }


        [HttpPost]
        public IActionResult ConfirmEmail(string key)
        {

            if (key != null && key.Equals(encryptclass.Decrypt(Request.Cookies["ActiveKey"], Request.Cookies["TempSlot"])))
            {
                string slot = Guid.NewGuid().ToString();
                CookieOptions cookie = new CookieOptions();
                var user = appData.User.SingleOrDefault(u => u.Id.Equals(encryptclass.Decrypt(Request.Cookies["TempId"], Request.Cookies["TempSlot"])));
                user.IsActive = true;
                appData.SaveChanges();
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Append("TempId", "1", cookie);
                Response.Cookies.Append("TempSlot", "1", cookie);
                Response.Cookies.Append("ActiveKey", "1", cookie);
                Response.Cookies.Append("id", encryptclass.Encrypt(user.Id, slot));
                Response.Cookies.Append("slot", slot);
            }
            else
            {
                ViewBag.ConfirmEamil = "Wrong key";
                return View();
            }
            return RedirectToAction("index", "Home");
        }

    }
}
