using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace FiveInLine.Dapr.Services
{
    public class StorageStateService : IStorageStateService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<StorageStateService> _logger;
        private readonly string _storageStateName;

        public StorageStateService(DaprClient daprClient, ILogger<StorageStateService> logger, string storageStateName)
        {
            _daprClient = daprClient;
            _logger = logger;
            _storageStateName = storageStateName;
        }

        private void CheckStorageStateName()
        {
            if (_storageStateName == null)
            {
                var message = "Storage state name is null. Please specify name before calling transaction methods.";
                _logger.LogError(message);

                throw new ArgumentNullException(message);
            }
        }

        public async Task<IEnumerable<T>> FilterFromState<T>(string query)
        {
            CheckStorageStateName();

            var response = await _daprClient.QueryStateAsync<T>(_storageStateName, query);

            return response.Results.Select(x => x.Data);
        }

        public async Task<T> GetFromState<T>(string key)
        {
            CheckStorageStateName();

            return await _daprClient.GetStateAsync<T>(_storageStateName, key);
        }

        public async Task SaveStateAsync<T>(string key, T value)
        {
            CheckStorageStateName();

            _logger.LogInformation("Saving key/value {@Event},{@Value}", key, value);
            await _daprClient.SaveStateAsync(_storageStateName, key, value);
        }
    }
}
