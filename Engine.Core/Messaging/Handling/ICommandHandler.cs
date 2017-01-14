using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.Messaging.Handling
{
    /// <summary>
    /// Marker Interface in order to descover handler through reflection
    /// More info see<see cref="http://blog.ploeh.dk/2011/03/22/CommandsareComposable/"/> 
    /// </summary>
    public interface ICommandHandler {}

    public interface ICommandHandler<T> : ICommandHandler
        where T : ICommand
    {
        void Handle(T command);
    }
}
