namespace FiveInLine.Dapr.Services
{
	public interface IStorageStateService
    {
        Task<T> GetFromState<T>(string key);
        Task<IEnumerable<T>> FilterFromState<T>(string query);
        Task SaveStateAsync<T>(string key, T value);
    }
}
