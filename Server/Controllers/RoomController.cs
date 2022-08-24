using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartChat.Shared;
using SmartChat.Shared.Models;
using SmartChat.Shared.ViewModels;

namespace SmartChat.Server.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _dbConnection;

        public RoomController(ILogger<RoomController> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;

            _dbConnection = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_dbConnection))
            {
                throw new Exception("Please provide database connection strings");
            }

            _userManager = userManager;
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRooms()
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var results = await conn.QueryAsync<Room>(@"SELECT Id, Name
                                                            FROM Rooms");
                return Ok(results);
            }
        }

        /// <summary>
        /// Returns a list of messages in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpGet("{roomId}/messages")]
        public async Task<IActionResult> GetRoomMessages(int roomId)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                var getQuery = @"SELECT Messages.Id, Messages.AuthorId, Messages.Content, Messages.Created, AspNetUsers.UserName AS AuthorName
                                 FROM Messages
                                 INNER JOIN RoomMessages ON Messages.Id = RoomMessages.MessageId
                                 INNER JOIN AspNetUsers ON AspNetUsers.Id = Messages.AuthorId
                                 WHERE RoomMessages.RoomId = @RoomId";

                var results = await conn.QueryAsync<ChatMessage>(getQuery, new {
                                    UserId = userId,
                                    RoomId = roomId
                                });

                return Ok(results);
            }
        }

        /// <summary>
        /// Subscribe to receive new messages posted in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost("{roomId}/subscribe")]
        public async Task<IActionResult> Subscribe(int roomId)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                // check if room exists
                var room = await conn.QuerySingleOrDefaultAsync<Room>(@"SELECT Id, Name FROM Rooms WHERE Id = @RoomId;", new
                {
                    RoomId = roomId
                });

                if (room == null)
                {
                    return BadRequest("Room does not exist");
                }

                var userId = _userManager.GetUserId(User);

                // check if room membership already exists
                var roomQuery = @"SELECT RoomId
                                  FROM RoomMembers
                                  WHERE UserId = @MyId";

                var membershipId = await conn.ExecuteScalarAsync<int?>(roomQuery, new { MyId = userId });

                // contact already exists
                if (membershipId.HasValue)
                {
                    return Ok(membershipId);
                }

                // if not add to room membership
                membershipId = await conn.ExecuteScalarAsync<int?>(@"INSERT INTO RoomMembers (UserId, RoomId)
                                                                     VALUES (@UserId, @RoomId)
                                                                     SELECT SCOPE_IDENTITY()", new { 
                                                                         RoomId = roomId,
                                                                         UserId = userId
                                                                     });

                if (!membershipId.HasValue)
                {
                    throw new Exception($"Unexpected response adding user: {userId} to room: {roomId}");
                }

                return Ok(membershipId);
            }      
        }

        /// <summary>
        /// Get all subscriptions for this user
        /// </summary>
        /// <returns></returns>
        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                var query = @"SELECT Rooms.Id, Rooms.Name
                              FROM RoomMembers
                              INNER JOIN Rooms ON Rooms.Id = RoomMembers.RoomId
                              WHERE UserId = @UserId";

                var results = await conn.QueryAsync<Room>(query, new { UserId = userId });

                return Ok(results);
            }
        }

        /// <summary>
        /// Create a new room with a name. The name must be unique
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("rooms")]
        public async Task<IActionResult> Rooms(string name)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                // check if room name already exists
                var room = await conn.QuerySingleOrDefaultAsync<Room>("SELECT Id, Name FROM Rooms WHERE Name = @Name;", new
                {
                    Name = name
                });

                if (room != null)
                {
                    return BadRequest("Room with name already exists");
                }

                // room name is unique so create room
                var roomId = await conn.ExecuteScalarAsync<int?>(@"INSERT INTO Rooms (Name)
                                                                   VALUES (@Name)
                                                                   SELECT SCOPE_IDENTITY()", new { Name = name });

                if (!roomId.HasValue)
                {
                    throw new Exception($"Failed inserting new room: {name}");
                }

                return Ok(roomId);
            }
        }

        /// <summary>
        /// Post a message in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost("{roomId}/messages")]
        public async Task<IActionResult> PostMessage(int roomId, [FromBody]string messageContent)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                if (String.IsNullOrEmpty(messageContent))
                {
                    return BadRequest("Message content is null or empty");
                }

                // check if room exists first
                var room = await conn.QuerySingleOrDefaultAsync<Room>("SELECT Id, Name FROM Rooms WHERE Id = @RoomId;", new
                {
                    RoomId = roomId
                });

                if (room == null)
                {
                    return BadRequest("Room does not exist");
                }

                var userId = _userManager.GetUserId(User);

                var insertQuery = @"INSERT INTO Messages (Content, Created, AuthorId)
                                    VALUES (@Content, GETUTCDATE(), @UserId)
                                    DECLARE @MessageId BIGINT
                                    SET @MessageId = SCOPE_IDENTITY()
                                    INSERT INTO RoomMessages (RoomId, MessageId)
                                    VALUES (@RoomId, @MessageId)
                                    SELECT Messages.Id FROM Messages WHERE Messages.Id = @MessageId";


                var messageId = await conn.ExecuteScalarAsync<Int64?>(insertQuery, new {
                        UserId = userId,
                        Content = messageContent,
                        RoomId = room.Id
                    });

                if (!messageId.HasValue)
                {
                    throw new Exception($"Unexpected response posting message to room: {room.Id}");
                }

                return Ok(messageId);
            }
        }
    }
}

