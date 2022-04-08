using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInLine.Dapr.EventPubSub
{
    public interface IEventPubSubProvider
    {
        IEventPubSub CreateEventPubSub(string pubsubName);
    }
}
