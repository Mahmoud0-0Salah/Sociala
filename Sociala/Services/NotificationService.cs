using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Hubs;
using Sociala.Migrations;
using Sociala.Models;

namespace Sociala.Services
{
    public interface INotification
    {
        public Task SendLikeNotification(string actorId, int postId);
        public Task SendCommentNotification(string actorId, int postId);
        
        public Task SendFriendRequestNotification(string actorId, string userId);
        public Task SendFriendRequestAcceptNotification(string actorId, string userId);

        public Task MarkAsRead(Notification notification);
        public Task HandleNotificationIcon(string userId);

    }
    public class NotificationService: INotification
    {
        private readonly AppData _data;
        private readonly IHubContext<NotificationsHub> _hubContext;
        public NotificationService(AppData context, IHubContext<NotificationsHub> hubContext)
        {
            _data = context;
            _hubContext = hubContext;
        }

        public async Task SendLikeNotification(string actorId, int postId)
        {
            var post = _data.Post.Where(p => p.Id == postId).SingleOrDefault();
            if (post == null || post.UserId == actorId)
            {
                return;
            }
            var actor = _data.User.FirstOrDefault(u => u.Id == actorId);
            var message = $"""{actor.UesrName} Liked Your Post""";
            var notification = new Notification() { UserId = post.UserId, Content=message, ActorId=actorId };
            _data.Notification.Add(notification);
            _data.SaveChanges();

            var dataToSend = new
            {
                id = notification.Id,
                message = message,
                userName = actor.UesrName,
                imgUrl = actor.UrlPhoto,
                createdAt = notification.CreatedAt,
                seen = notification.Seen,
            };
            string jsonData = JsonSerializer.Serialize(dataToSend);
            await SendMessageToUser(post.UserId, jsonData);
            await HandleNotificationIcon(post.UserId);
        }

        public async Task SendCommentNotification(string actorId, int postId)
        {

        }
        public async Task SendFriendRequestNotification(string actorId, string userId)
        {
            var actor = _data.User.FirstOrDefault(u => u.Id == actorId);
            var message = $"""{actor.UesrName} Sent you a friend request""";
            var notification = new Notification() { UserId = userId, Content = message, ActorId = actorId };
            _data.Notification.Add(notification);
            _data.SaveChanges();

            var dataToSend = new
            {
                id = notification.Id,
                message = message,
                userName = actor.UesrName,
                imgUrl = actor.UrlPhoto,
                createdAt = notification.CreatedAt,
                seen = notification.Seen,
            };
            string jsonData = JsonSerializer.Serialize(dataToSend);
            await SendMessageToUser(userId, jsonData);
            await HandleNotificationIcon(userId);
        }

        public async Task SendFriendRequestAcceptNotification(string actorId, string userId)
        {
            var actor = _data.User.FirstOrDefault(u => u.Id == actorId);
            var message = $"""{actor.UesrName} Accept your request""";
            var notification = new Notification() { UserId = userId, Content = message, ActorId = actorId };
            _data.Notification.Add(notification);
            _data.SaveChanges();

            var dataToSend = new
            {
                id = notification.Id,
                message = message,
                userName = actor.UesrName,
                imgUrl = actor.UrlPhoto,
                createdAt = notification.CreatedAt,
                seen = notification.Seen,
            };
            string jsonData = JsonSerializer.Serialize(dataToSend);
            await SendMessageToUser(userId, jsonData);
            await HandleNotificationIcon(userId);
        }


        public async Task MarkAsRead(Notification notification)
        {


            notification.Seen = true;
            _data.SaveChanges();
            var userId = notification.UserId;
            var _userConnections = NotificationsHub.GetUsersConnections();
            if (userId != null && _userConnections.TryGetValue(userId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("MarkAsRead", notification.Id);
                }
            }
            await HandleNotificationIcon(userId);

        }

        public async Task SendMessageToUser(string userId, string message)
        {
            var _userConnections = NotificationsHub.GetUsersConnections();
            if (userId != null && _userConnections.TryGetValue(userId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                }
            }
        }
        public async Task HandleNotificationIcon(string userId)
        {
            var unSeenNotifications = _data.Notification.Where(n => n.Seen == false && n.UserId == userId).Count();
            if (unSeenNotifications > 0)
            {
                await ActivateNotificationIcon(userId);
            }
            else
            {
                await DeActivateNotificationIcon(userId);
            }
        }
        private async Task ActivateNotificationIcon(string userId) {

            var _userConnections = NotificationsHub.GetUsersConnections();
            if (userId != null && _userConnections.TryGetValue(userId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ActivateNotificationIcon");
                }
            }

        }

        private async Task DeActivateNotificationIcon(string userId) {
            var _userConnections = NotificationsHub.GetUsersConnections();
            if (userId != null && _userConnections.TryGetValue(userId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("DeActivateNotificationIcon");
                }
            }
        }

    }
}
