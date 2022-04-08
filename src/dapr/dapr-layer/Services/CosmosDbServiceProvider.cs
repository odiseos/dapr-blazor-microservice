using Microsoft.Azure.Cosmos;
using FiveInLine.Dapr.Infrastructure;

namespace FiveInLine.Dapr.Services
{
    public class CosmosDbServiceProvider : ICosmosDbServiceProvider
    {
        private readonly CosmosDbSettings _settings;
        private readonly Dictionary<string, ICosmosDbService> _comosDbServices;

        public CosmosDbServiceProvider(CosmosDbSettings settings)
        {
            _settings = settings;
            _comosDbServices = new Dictionary<string, ICosmosDbService>();
        }

        public ICosmosDbService CreateCosmosDbService(string containerName)
        {
            return GetCosmosDbService(containerName) ?? MakeCosmosDbService(containerName);
        }

        private ICosmosDbService MakeCosmosDbService(string containerName)
        {
            // Create cosmos client
            var cosmosClient = new CosmosClient(_settings.Url, _settings.MasterKey);

            //create repository
            var cosmosDbService = new CosmosDbService(cosmosClient, _settings.DatabaseName, containerName);

            //insert repository in dictionary
            _comosDbServices[containerName] = cosmosDbService;

            return cosmosDbService;
        }

        private ICosmosDbService? GetCosmosDbService(string containerName)
        {
            if (_comosDbServices.TryGetValue(containerName, out ICosmosDbService? cosmosDbService))
                return cosmosDbService;

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_comosDbServices != null)
                {
                    _comosDbServices.Clear();
                }
            }
            _disposed = true;
        }
    }
}
