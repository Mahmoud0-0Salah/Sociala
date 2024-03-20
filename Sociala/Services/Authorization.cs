using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;
using Microsoft.AspNetCore.Http;
using EncryptServices;

namespace AuthorizationService
{
    public interface IAuthorization
    {
        public bool IsLoggedIn();
        public bool IsAdmin(string id);
        public bool IsUser(string id);
        public string GetId();
        
    }
    public class Authorization :IAuthorization
    {
        private readonly AppData appData;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncrypt encryptclass;
        public Authorization(AppData appData, IHttpContextAccessor httpContextAccessor, IEncrypt encryptclass)
        {
            this.appData = appData;
            _httpContextAccessor = httpContextAccessor;
            this.encryptclass = encryptclass;
        }

        public bool IsLoggedIn()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies["id"] != null;
        }
        public bool IsAdmin(string id)
        {
            id = encryptclass.Decrypt(id, _httpContextAccessor.HttpContext.Request.Cookies["slot"]);
            var user =appData.User.Where( u => u.Id.Equals(id)).SingleOrDefault();
            return user.RoleId == 1;
        }
        public bool IsUser(string id)
        {
            id = encryptclass.Decrypt(id, _httpContextAccessor.HttpContext.Request.Cookies["slot"]);
            var user = appData.User.Where(u => u.Id.Equals(id)).SingleOrDefault();
            return user.RoleId == 0;
        }
        public string GetId()
        {
            return encryptclass.Decrypt(_httpContextAccessor.HttpContext.Request.Cookies["id"], _httpContextAccessor.HttpContext.Request.Cookies["slot"]);
        }
    }
}
