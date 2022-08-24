using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using SmartChat.Shared.Models;
using SmartChat.Shared.ViewModels;
using System.Security.Claims;

namespace SmartChat.Server
{
    [Authorize]
    public class SignalRHub : Hub
    {
        private readonly ILogger<SignalRHub> _logger;
        private readonly string _dbConnection;

        public SignalRHub(ILogger<SignalRHub> logger, IConfiguration configuration)
        {
            _logger = logger;

            _dbConnection = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_dbConnection))
            {
                throw new Exception("Please provide database connection strings");
            }
        }

        /// <summary>
        /// Send message to all connections with the user id
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public async Task BroadcastMessageAsync(ChatMessage message, string toUserId)
        {
            await Clients.User(toUserId).SendAsync("ChatNotification", message);
        }

        /// <summary>
        /// Send message to all other connections that are subscribed to this room
        /// </summary>
        /// <param name="message"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task BroadcastRoomMessageAsync(ChatMessage message, int roomId)
        {
            IEnumerable<string> subscribers = new List<string>();

            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var query = @"SELECT RoomMembers.UserId
                              FROM RoomMembers
                              INNER JOIN AspNetUsers ON AspNetUsers.Id = RoomMembers.UserId
                              WHERE RoomMembers.RoomId = @RoomId AND RoomMembers.UserId != @UserId";

                // everyone but the caller of this method
                subscribers = await conn.QueryAsync<string>(query, new { RoomId = roomId, UserId = (String.IsNullOrEmpty(userId)) ? string.Empty : userId });

                var roomName = await conn.ExecuteScalarAsync<string>(@"SELECT Rooms.Name FROM Rooms WHERE Rooms.Id = @RoomId", new { RoomId = roomId });

                foreach (var subscriber in subscribers)
                {
                    await Clients.User(subscriber).SendAsync("RoomNotification", message, new Room { Id = roomId, Name = roomName });
                }
            }
        }
    }
}
