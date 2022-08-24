using SmartChat.Shared.Models;
using SmartChat.Shared.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartChat.Client
{
    public class SmartChatService
    {
        private readonly IHttpClientFactory _clientFactory;

        public SmartChatService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Return all chat rooms present in the application
        /// </summary>
        /// <returns></returns>
        public async Task<List<Room>> GetAllChatRoomsAsync()
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var results = await client.GetFromJsonAsync<List<Room>>($"/room/all");

            return results;
        }

        /// <summary>
        /// Get all rooms that this user is subscribed to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Room>> GetAllRoomSubscriptions()
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var results = await client.GetFromJsonAsync<List<Room>>($"/room/subscriptions");

            return results;
        }

        /// <summary>
        /// Subscribe this user to receive room notifications for a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task<int> SubscribeToRoom(int roomId)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsync($"/room/{roomId}/subscribe", new StringContent(""));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Convert.ToInt32(content);
            }

            return 0;
        }

        /// <summary>
        /// Create a chat room
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<int> AddRoomAsync(string name)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsync($"/room/rooms?name={name}", new StringContent(""));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Convert.ToInt32(content);
            }

            return 0;
        }

        public async Task<SmartContact> AddContactAsync(string name)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsync($"/chat/{name}/contacts", new StringContent(""));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var contact = JsonSerializer.Deserialize<SmartContact>(content);

                return contact;
            }

            return null;
        }

        /// <summary>
        /// Get all people we are talking to
        /// </summary>
        /// <returns></returns>
        public async Task<List<SmartContact>> GetAllContactsAsync()
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            return await client.GetFromJsonAsync<List<SmartContact>>($"/chat/contacts");
        }

        /// <summary>
        /// Get all messages in a conversation
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public async Task<List<ChatMessage>> GetConversationAsync(long conversationId)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            return await client.GetFromJsonAsync<List<ChatMessage>>($"/chat/{conversationId}/messages");
        }

        /// <summary>
        /// Post a message to a user
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        public async Task<long> SaveMessageAsync(long conversationId, string messageContent)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsJsonAsync<string>($"/chat/{conversationId}/messages", messageContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Int64.Parse(content);
            }

            return 0;
        }

        /// <summary>
        /// Get all messages in a room
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public async Task<List<ChatMessage>> GetRoomMessagesAsync(int roomId)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            return await client.GetFromJsonAsync<List<ChatMessage>>($"/room/{roomId}/messages");
        }

        /// <summary>
        /// Post a message to a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        public async Task<int> SaveRoomMessageAsync(int roomId, string messageContent)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsJsonAsync<string>($"/room/{roomId}/messages", messageContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Int32.Parse(content);
            }

            return 0;
        }
    }
}
