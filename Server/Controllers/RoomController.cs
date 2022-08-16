using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartChat.Shared;

namespace SmartChat.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;

        public RoomController(ILogger<RoomController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of messages in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpGet("{roomId}/messages")]
        public async Task<IActionResult> Messages(int roomId)
        {
            return Ok();
        }

        /// <summary>
        /// Subscribe to receive new messages posted in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpGet("{roomId}/subscribe")]
        public async Task<IActionResult> Subscribe(int roomId)
        {
            return Ok();
        }

        /// <summary>
        /// Create a new room with a name. The name must be unique
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("rooms")]
        public async Task<IActionResult> Rooms(string name)
        {
            return Ok();
        }

        /// <summary>
        /// Post a message in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost("{roomId}/messages")]
        public async Task<IActionResult> PostMessage(int roomId)
        {
            return Ok();
        }
    }
}

