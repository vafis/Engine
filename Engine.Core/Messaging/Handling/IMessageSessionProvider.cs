using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.Messaging.Handling
{
    public interface IMessageSessionProvider
    {
        string SessionId { get; }
    }
}
