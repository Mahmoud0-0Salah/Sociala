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
        public IActionResult Index()
        {
            return View();
        }
    }
}
