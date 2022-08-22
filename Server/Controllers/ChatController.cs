using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartChat.Shared.Models;

namespace SmartChat.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _dbConnection;

        public ChatController(ILogger<ChatController> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager)
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
        /// Send message to user from another user
        /// </summary>
        /// <param name="ToUserId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("{ToUserId}/messages")]
        public async Task<IActionResult> PostMessage(string ToUserId, [FromBody] string messageContent)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                if (String.IsNullOrEmpty(messageContent))
                {
                    return BadRequest("Message content is null or empty");
                }

                // check if user sending message to exists first
                var recipient = await _userManager.FindByIdAsync(ToUserId);

                if (recipient == null)
                {
                    return BadRequest("User sending message to doesn't exist");
                }

                var authorId = _userManager.GetUserId(User);

                var insertedRows = await conn.ExecuteAsync("INSERT INTO Messages (Content, Created, AuthorId)" +
                                                          "VALUES (@Content, GETUTCDATE() @AuthorId)" +
                                                          "INSERT INTO DirectMessages (ToUserId, FromUserId, MessageId)" +
                                                          "VALUES (@ToUserId, @FromUserId, SCOPE_IDENTITY()", new
                                                          {
                                                              AuthorId = authorId,
                                                              Content = messageContent,
                                                              ToUserId = recipient.Id,
                                                              FromUserId = authorId
                                                          });

                if (insertedRows == 0)
                {
                    throw new Exception($"Unexpected response sending message to user: {ToUserId}");
                }

                return Ok();
            }
        }

        /// <summary>
        /// Get all direct message conversations of this user
        /// </summary>
        /// <returns></returns>
        [HttpGet("/messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                var results = await conn.QueryAsync("SELECT Messages.Id, UserName, Content" +
                                                    "FROM Messages" +
                                                    "INNER JOIN DirectMessages ON DirectMessages.FromUserId = Messages.AuthorId" +
                                                    "INNER JOIN DirectMessage ON DirectMessages.ToUserId = Messages.AuthorId" +
                                                    "INNER JOIN AspNetUsers ON AspNetUsers.Id = DirectMessages.FromUserId" +
                                                    "WHERE Messages.AuthorId = @AuthorId", new
                                                    {
                                                        AuthorId = userId
                                                    });
                return Ok(results);
            }
        }

        /// <summary>
        /// Get conversation history between this user and another user
        /// </summary>
        /// <param name="ToUserId"></param>
        /// <returns></returns>
        [HttpGet("{ToUserId}/messages")]
        public async Task<IActionResult> GetMessages(string ToUserId)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                var results = await conn.QueryAsync("SELECT Messages.Id, UserName, Content" +
                                                    "FROM Messages" +
                                                    "INNER JOIN DirectMessages ON DirectMessages.FromUserId = Messages.AuthorId" + 
                                                    "INNER JOIN AspNetUsers ON AspNetUsers.Id = DirectMessages.FromUserId" +
                                                    "WHERE Messages.AuthorId = @AuthorId", new
                                                    {
                                                        AuthorId = userId
                                                    });
                return Ok(results);
            }
        }
    }
}
