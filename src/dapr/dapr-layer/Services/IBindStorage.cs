using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.Services
{
    public interface IBindStorage
    {
        Task<T> Get<T>(string blobName);
        Task<T> Create<T>(string blobName, string data);
        string GetBlobName(string url);
    }
}
