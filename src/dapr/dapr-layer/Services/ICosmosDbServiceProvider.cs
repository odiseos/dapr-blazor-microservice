using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.Services
{
    public interface ICosmosDbServiceProvider : IDisposable
    {
        ICosmosDbService CreateCosmosDbService(string containerName);
    }
}
