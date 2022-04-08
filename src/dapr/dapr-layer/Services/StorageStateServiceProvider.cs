using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.Services
{
    public class StorageStateServiceProvider : IStorageStateServiceProvider
    {
        private readonly DaprClient _daprClient;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, IStorageStateService> _storageStateServices;

        public StorageStateServiceProvider(DaprClient daprClient, ILoggerFactory loggerFactory)
        {
            _daprClient = daprClient;
            _loggerFactory = loggerFactory;
            _storageStateServices = new ConcurrentDictionary<string, IStorageStateService>();
        }

        public IStorageStateService CreateStorageStateService(string storageStateName)
        {
            return GetStorageStateService(storageStateName) ?? MakeStorageStateService(storageStateName);
        }

        private IStorageStateService MakeStorageStateService(string storageStateName)
        {
            //create repository
            var storageStateService = new StorageStateService(_daprClient, _loggerFactory.CreateLogger<StorageStateService>(), storageStateName);

            //insert repository in dictionary
            _storageStateServices[storageStateName] = storageStateService;

            return storageStateService;
        }

        private IStorageStateService? GetStorageStateService(string storageStateName)
        {
            if (_storageStateServices.TryGetValue(storageStateName, out IStorageStateService? storageStateService))
                return storageStateService;

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
                if (_storageStateServices != null)
                {
                    _storageStateServices.Clear();
                }
            }
            _disposed = true;
        }
    }
}
