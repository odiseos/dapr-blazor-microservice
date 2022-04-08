namespace BlazorUI.Clients
{
    public class UserClient
    {
        private const int PageSize = 12;

        private readonly HttpClient httpClient;

        public UserClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<User>> GetAsync() => httpClient.GetFromJsonAsync<IEnumerable<User>>("u/api/v1/user")!;
    }
}
