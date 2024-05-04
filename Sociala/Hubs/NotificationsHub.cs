using System.Net;
using System.Text.Json;
using AuthorizationService;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Models;
using Sociala.Services;

namespace Sociala.Hubs
{
    public class NotificationsHub: Hub
    {
        private static readonly Dictionary<string, HashSet<string>> _userConnections = new Dictionary<string, HashSet<string>>();
        private readonly IAuthorization _authorization;
        private readonly AppData _appData;
        private readonly INotification _notificationService;
        public NotificationsHub(IAuthorization authorization, AppData appData, INotification notificationService)
        {
            _authorization = authorization;
            _appData = appData;
            _notificationService = notificationService;
        }
        public override Task OnConnectedAsync()
        {
            string userId = _authorization.GetId();
            string connectionId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(userId))
            {
                lock (_userConnections)
                {
                    if (!_userConnections.ContainsKey(userId))
                    {
                        _userConnections[userId] = new HashSet<string>();
                    }

                    _userConnections[userId].Add(connectionId);
                }
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userId = _authorization.GetId();
            string connectionId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(userId))
            {
                lock (_userConnections)
                {
                    if (_userConnections.ContainsKey(userId))
                    {
                        _userConnections[userId].Remove(connectionId);

                        if (_userConnections[userId].Count == 0)
                        {
                            _userConnections.Remove(userId);
                        }
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        
        public async Task SendAllNotifications()
        {
            
            string userId = _authorization.GetId();
            if (userId == null)
            {
                return;
            }
            var notifications = _appData.Notification.Where(n => n.UserId == userId).Include(n => n.Actor)
                                                     .OrderByDescending(n => n.CreatedAt).Take(5).ToList();
            var notificationList = new List<string>();
            foreach (var notification in notifications)
            {
                var dataToSend = new
                {
                    id = notification.Id,
                    message = notification.Content,
                    userName = notification.Actor.UesrName,
                    imgUrl = notification.Actor.UrlPhoto,
                    createdAt = notification.CreatedAt,
                    seen = notification.Seen,
                };  // add a link to the post
                string jsonData = JsonSerializer.Serialize(dataToSend);
                notificationList.Add(jsonData);
            }

            await Clients.Caller.SendAsync("ReceiveNotificationList", notificationList);
            await _notificationService.HandleNotificationIcon(userId);

        }

        public static Dictionary<string, HashSet<string>> GetUsersConnections()
        {
            return _userConnections;
        }

    }

}
