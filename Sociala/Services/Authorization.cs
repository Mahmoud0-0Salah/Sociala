using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;
using Microsoft.AspNetCore.Http;
using EncryptServices;
using AuthorizationService;
using Sociala.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AuthorizationService
{
    public interface IAuthorization
    {
        public bool IsLoggedIn();
        public bool IsAdmin(string id);
        public bool IsUser(string id);
        public string GetId();
        public bool IsBanned(string id);


    }
    public class Authorization :IAuthorization
    {
        private readonly AppData appData;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncrypt encryptclass;
        private readonly IConfiguration configuration;
        public Authorization(AppData appData, IHttpContextAccessor httpContextAccessor, IEncrypt encryptclass, IConfiguration configuration)
        {
            this.appData = appData;
            _httpContextAccessor = httpContextAccessor;
            this.encryptclass = encryptclass;
            this.configuration = configuration;
        }

        public bool IsLoggedIn()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies["id"] != null;
        }
        public bool IsAdmin(string id)
        {
            var user =appData.User.Where( u => u.Id.Equals(id)).SingleOrDefault();
            var role= appData.Role.Where(r => r.Id.Equals(user.RoleId)).SingleOrDefault();
            return role.Name.Equals("Admin");
        }
        public bool IsUser(string id)
        {
            var user = appData.User.Where(u => u.Id.Equals(id)).SingleOrDefault();
            var role = appData.Role.Where(r => r.Id.Equals(user.RoleId)).SingleOrDefault();
            return role.Name.Equals("User");
        }
        public bool IsBanned(string id)
        {
            var user = appData.User.Where(u => u.Id.Equals(id)).SingleOrDefault();
            return user.IsBanned;
        }

        public string GetId()
        {
            return encryptclass.Decrypt(_httpContextAccessor.HttpContext.Request.Cookies["id"], configuration.GetSection("Key").ToString());
        }
    }
}
