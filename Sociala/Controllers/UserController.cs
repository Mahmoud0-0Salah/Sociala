using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Sociala.Data;
using Sociala.Models;
using Sociala.ViewModel;
using EncryptServices;
using EmailSendertServices;

namespace Sociala.Controllers
{
    public class UserController : Controller
    {
        private readonly AppData appData;
        private readonly IEmailSender emailSender;
        private readonly IEncrypt encryptclass;
        public UserController(AppData appData, IEncrypt encryptClass, IEmailSender emailSender)
        {
            this.appData = appData;
            this.encryptclass = encryptClass;
            this.emailSender = emailSender;
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
            if (Request.Cookies["id"] != null)
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
            user.ActiveKey = Guid.NewGuid().ToString();
            user.CreateAt= DateTime.Now;
            try
            {
                await emailSender.SendEmailAsync(user.Email, "Confirm email", $"Hello {user.UesrName}\n\nYou're almost there!\r\nPlease confirm your subscription by enter this key \n{user.ActiveKey}");

                appData.Add(user);
                await appData.SaveChangesAsync();
                string slot = Guid.NewGuid().ToString();
                Response.Cookies.Append("ActiveKey", encryptclass.Encrypt(user.ActiveKey, slot));
                Response.Cookies.Append("TempId", encryptclass.Encrypt(user.Id, slot));
                Response.Cookies.Append("TempSlot", slot);
            }
            catch
            {
                return Content("Bad connection please try again");
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
