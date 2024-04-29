using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Hubs;
using Sociala.Models;

namespace Sociala.Services
{
    public interface INotification
    {
        public Task SendLikeNotification(string actorId, int postId);
        public Task SendCommentNotification(string actorId, int postId);

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
            if (post == null)
            {
                return;
            }
            var actor = _data.User.FirstOrDefault(u => u.Id == actorId);
            var message = $"""{actor.UesrName} Liked Your Post""";
            var notification = new Notification() { UserId = post.UserId, Content=message };
            _data.Notification.Add(notification);
            _data.SaveChanges();

            var dataToSend = new
            {
                message = message,
                userName = actor.UesrName,
                imgUrl = actor.UrlPhoto
            };  // add a link to the post
            string jsonData = JsonSerializer.Serialize(dataToSend);
            await SendMessageToUser(actorId, jsonData);
        }

        public async Task SendCommentNotification(string actorId, int postId)
        {

        }

        public async Task SendMessageToUser(string userId, string message)
        {
            var _userConnections = NotificationsHub.GetUsersConnections();
            if (!string.IsNullOrEmpty(userId) && _userConnections.TryGetValue(userId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                }
            }
        }

    }
}
