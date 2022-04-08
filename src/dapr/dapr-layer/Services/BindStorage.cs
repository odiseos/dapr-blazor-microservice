using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.Services
{
    public class BindStorage : IBindStorage
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<BindStorage> _logger;
        private string _bindStorageName;
        private bool _isBlob;
        private string Metadata  => _isBlob ? "blobName" : "fileName";

        public BindStorage(DaprClient daprClient, ILogger<BindStorage> logger, string bindStorageName, bool isBlob = true)
        {
            _daprClient = daprClient;
            _logger = logger;
            _bindStorageName = bindStorageName;
            _isBlob = isBlob;
        }

        private void CheckStorageStateName()
        {
            if (_bindStorageName == null)
            {
                var message = "Bind storage name is null. Please specify name before calling transaction methods.";
                _logger.LogError(message);

                throw new ArgumentNullException(message);
            }
        }

        public async Task<T> Get<T>(string blobName)
        {
            CheckStorageStateName();

            var blobContaine = await _daprClient.InvokeBindingAsync<string, T>(_bindStorageName, "get", "", new Dictionary<string, string> { { Metadata, blobName } });

            return blobContaine;
        }

        public async Task<T> Create<T>(string blobName, string data)
        {
            CheckStorageStateName();

            var blobContaine = await _daprClient.InvokeBindingAsync<string, T>(_bindStorageName, "create", data, new Dictionary<string, string> { { Metadata, blobName } });

            return blobContaine;
        }

        public string GetBlobName(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var urlSplitted = url.Split('/');
            var length = urlSplitted.Length;
            return length > 0 ? urlSplitted[length - 1] : string.Empty;
        }
    }
}
