using System.Net;
using AuthorizationService;
using Microsoft.AspNetCore.SignalR;
using Sociala.Models;

namespace Sociala.Hubs
{
    public class NotificationsHub: Hub
    {
        private static readonly Dictionary<string, HashSet<string>> _userConnections = new Dictionary<string, HashSet<string>>();
        private readonly IAuthorization _authorization;
        public NotificationsHub(IAuthorization authorization)
        {
            _authorization = authorization;
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

        
        public async Task SendMessage(string message)
        {
            string allUsers = "";
            foreach (var user in _userConnections)
            {
                allUsers += ", " + user;
            }
            await Clients.All.SendAsync("ReceiveMessage", allUsers);
        }

        public static Dictionary<string, HashSet<string>> GetUsersConnections()
        {
            return _userConnections;
        }

    }

}
