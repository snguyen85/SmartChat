using SmartChat.Shared.Models;
using SmartChat.Shared.ViewModels;
using System.Net.Http.Json;

namespace SmartChat.Client
{
    public class SmartChatService
    {
        private readonly IHttpClientFactory _clientFactory;

        public SmartChatService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<List<Room>> GetAllChatRoomsAsync()
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.GetAsync($"/room/all");
            var content = await response.Content.ReadAsStringAsync();

            var results = await client.GetFromJsonAsync<List<Room>>($"/room/all");

            return results;
        }

        public async Task SubscribeToRoom(int roomId)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsync($"/room/{roomId}/subscribe", new StringContent(""));

            if (response.IsSuccessStatusCode)
            {

            }
        }

        public async Task<Room> AddChatRoomAsync(string name)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsync($"/room/rooms?name={name}", new StringContent(""));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new Room
                {
                    Id = Convert.ToInt32(content),
                    Name = name,
                };
            }

            throw new Exception($"{content}");
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

        public async Task<List<ChatMessage>> GetConversationAsync(long conversationId)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            return await client.GetFromJsonAsync<List<ChatMessage>>($"/chat/{conversationId}/messages");
        }

        public async Task<ApplicationUser> GetUserDetailsAsync(string userId)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            return await client.GetFromJsonAsync<ApplicationUser>($"/chat/users/{userId}");
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var data = await client.GetFromJsonAsync<List<ApplicationUser>>("/chat/users");
            
            return data;
        }

        public async Task SaveMessageAsync(long conversationId, string messageContent)
        {
            var client = _clientFactory.CreateClient("SmartChat.Server");

            var response = await client.PostAsJsonAsync<string>($"/chat/{conversationId}/messages", messageContent);
            var content = await response.Content.ReadAsStringAsync();
        }
    }
}
