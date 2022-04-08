namespace FiveInLine.Dapr.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<T>> GetQueryAsync<T>(string queryString);
    }
}
