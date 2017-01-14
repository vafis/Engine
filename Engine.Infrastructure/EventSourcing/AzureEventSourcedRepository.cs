using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.EventSourcing;

namespace Engine.Infrastructure.EventSourcing
{
    /// <summary>
    /// Defines a repository of <see cref="IEventSourced"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AzureEventSourcedRepository<T> : IEventSourcedRepository<T> where T:class, IEventSourced
    {
        public T Find(Guid id)
        {
            throw new NotImplementedException();
        }

        public T Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Save(T eventSoursed, string corellationId)
        {
            throw new NotImplementedException();
        }
    }
}
