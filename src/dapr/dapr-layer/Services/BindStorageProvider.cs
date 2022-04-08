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
    public class BindStorageProvider: IBindStorageProvider
    {
        private readonly DaprClient _daprClient;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, IBindStorage> _bingStorages;

        public BindStorageProvider(DaprClient daprClient, ILoggerFactory loggerFactory)
        {
            _daprClient = daprClient;
            _loggerFactory = loggerFactory;
            _bingStorages = new ConcurrentDictionary<string, IBindStorage>();
        }

        public IBindStorage CreateBingStorage(string bindStorageName, bool isBlob = true)
        {
            return GetBingStorage(bindStorageName) ?? MakeBingStorage(bindStorageName, isBlob);
        }

        private IBindStorage MakeBingStorage(string bindStorageName, bool isBlob = true)
        {
            //create repository
            var storageStateService = new BindStorage(_daprClient, _loggerFactory.CreateLogger<BindStorage>(), bindStorageName, isBlob);

            //insert repository in dictionary
            _bingStorages[bindStorageName] = storageStateService;

            return storageStateService;
        }

        private IBindStorage? GetBingStorage(string bindStorageName)
        {
            if (_bingStorages.TryGetValue(bindStorageName, out IBindStorage? bindStorage))
                return bindStorage;

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
                if (_bingStorages != null)
                {
                    _bingStorages.Clear();
                }
            }
            _disposed = true;
        }
    }
}
