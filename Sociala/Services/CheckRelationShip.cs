﻿using Microsoft.AspNetCore.Mvc;
using Sociala.Data;
using Sociala.Models;
using Microsoft.AspNetCore.Http;
using EncryptServices;
using AuthorizationService;
using Sociala.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;

namespace Sociala.Services
{
    public interface ICheckRelationShip
    {
        public bool IsFriend( string UserId);
        public bool IsRequested( string UserId);
        public bool IsRequesting( string UserId);
        public bool IsBlock(string UserId);
        public bool IsMe( string UserId);
       

    }
    public class CheckRelationShip: ICheckRelationShip
    {
        private readonly AppData appData;
        private readonly IAuthorization authorization;

        public CheckRelationShip(AppData appData, IAuthorization authorization)
        {
            this.authorization = authorization;
            this.appData = appData; 
        }
        public bool IsFriend(string UserId)
        {
            string id = authorization.GetId();
            var Result = appData.Friend
                .Where(p => (p.RequestingUserId == id && p.RequestedUserId == UserId) || (p.RequestingUserId == UserId && p.RequestedUserId == id))
                .ToList(); 
            return Result.Count() > 0;
        }
        public bool IsRequested( string UserId)
        {
            string id = authorization.GetId();
            var Result = appData.Request.Where(p => (p.RequestingUserId ==id && p.RequestedUserId == UserId) ).ToList();
            
            return Result.Count() > 0;

        }

        public bool IsRequesting(string UserId)
        {
            string id = authorization.GetId();
            var Result = appData.Request.Where(p => (p.RequestingUserId == UserId  && p.RequestedUserId == id)).ToList();

            return Result.Count() > 0;

        }
        public bool IsBlock(string UserId) ///انا اللي عملت بلوك
        {
            string Id = authorization.GetId();
            var Result = appData.Block.Where(p=>(p.Blocking==Id&&p.Blocked==UserId) ||(p.Blocking == UserId && p.Blocked == Id)).ToList();

            return Result.Count() > 0;

        }
       

        public bool IsMe(string UserId)
        {
            return UserId== authorization.GetId();
        }


    }
}
