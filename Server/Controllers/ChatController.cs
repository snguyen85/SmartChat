using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartChat.Shared.Models;
using SmartChat.Shared.ViewModels;

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

                // get conversation if it exists

                // no conversation yet so created one

                var insertedRows = await conn.ExecuteAsync(@"INSERT INTO Messages (Content, Created, AuthorId)
                                                             VALUES (@Content, GETUTCDATE() @AuthorId)
                                                             INSERT INTO DirectMessages (ToUserId, FromUserId, MessageId)
                                                             VALUES (@ToUserId, @FromUserId, SCOPE_IDENTITY()", new
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
        /// Get all people this user is talking to
        /// </summary>
        /// <returns></returns>
        [HttpGet("contacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                var conversationQuery = @"SELECT ConversationId
                                          FROM UserConversations
                                          WHERE UserId = @UserId";

                var myConversations = await conn.QueryAsync<int>(conversationQuery, new { UserId = userId });

                var usersQuery = @"SELECT UserConversations.UserId AS Id, UserConversations.ConversationId, AspNetUsers.UserName As Username
                                   FROM UserConversations
                                   INNER JOIN AspNetUsers ON AspNetUsers.Id = UserConversations.UserId
                                   WHERE UserConversations.ConversationId IN @ConversationIds AND UserId != @MyId";

                var results = await conn.QueryAsync<SmartContact>(usersQuery, new { MyId = userId, ConversationIds = myConversations });

                return Ok(results);
            }
        }

        /// <summary>
        /// Get all direct message conversations of this user
        /// </summary>
        /// <returns></returns>
        [HttpGet("messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                var query = @"SELECT ConversationId
                              FROM UserConversations
                              WHERE UserId = @UserId";

                var conversations = await conn.QueryAsync<int>(query, new { UserId = userId });


                var messageQuery = @"SELECT Messages.Id, Messages.AuthorId, Messages.Content, Messages.Created, DirectMessages.ConversationId
                                     FROM Messages
                                     INNER JOIN DirectMessages ON DirectMessages.MessageId = Messages.Id
                                     WHERE DirectMessages.ConversationId IN @ConversationIds
                                     ORDER BY DirectMessages.ConversationId, Messages.Id";

                var conversationMessages = await conn.QueryAsync<ChatMessage>(messageQuery, new { ConversationIds = conversations });

                return Ok(conversationMessages);
            }
        }

        /// <summary>
        /// Get conversation history for this user at the specified conversation
        /// id
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(long conversationId)
        {
            using (var conn = new SqlConnection(_dbConnection))
            {
                await conn.OpenAsync();

                var userId = _userManager.GetUserId(User);

                // get conversation if it exists
                var query = @"SELECT * 
                              FROM UserConversations
                              WHERE ConversationId = @ConversationId AND UserId = @UserId";

                var conversation = await conn.QuerySingleOrDefaultAsync<UserConversation>(query, new { ConversationId = conversationId, UserId = userId });

                if (conversation == null)
                {
                    return BadRequest("No conversation has been started");
                }

                var conversationsQuery = @"SELECT Messages.Id, Messages.AuthorId, Messages.Content, Messages.Created, AspNetUsers.UserName AS AuthorName
                                           FROM Messages
                                           INNER JOIN DirectMessages ON DirectMessages.MessageId = Messages.Id
                                           INNER JOIN AspNetUsers ON AspNetUsers.Id = Messages.AuthorId
                                           WHERE DirectMessages.ConversationId = @ConversationId";
                
                var conversations = await conn.QueryAsync<ChatMessage>(conversationsQuery, new { ConversationId = conversation.Id });

                return Ok(conversations);
            }
        }
    }
}
