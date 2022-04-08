using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace BlazorUI.Clients
{
    public class GameClient
    {
        private const int PageSize = 12;

        private readonly HttpClient httpClient;

        public GameClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<PlayedGame>> GetAsync() => httpClient.GetFromJsonAsync<IEnumerable<PlayedGame>>("g/api/v1/game")!;

        public async Task<HttpResponseMessage> PostAsync(GameMessage message)
        {
            try
            {
                var messageJson = new StringContent(
                               JsonSerializer.Serialize(message),
                               Encoding.UTF8,
                               Application.Json);

                var httpResponseMessage = await httpClient.PostAsync("g/api/v1/game", messageJson);
                return httpResponseMessage;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
