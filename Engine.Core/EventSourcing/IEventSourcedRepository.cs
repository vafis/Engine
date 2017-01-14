using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.EventSourcing
{
    public interface IEventSourcedRepository<T> where T : IEventSourced
    {
        /// <summary>
        /// Tries to retrive the event sourced entity
        /// </summary>
        /// <param name="Id">The Id of the entity</param>
        /// <returns>The hydraded entity of null if does not exist</returns>
        T Find(Guid id);
        T Get(Guid id);
        /// <summary>
        /// Save the event sourced entity  
        /// </summary>
        /// <param name="eventSoursed"></param>
        /// <param name="corellationId"></param>
        void Save(T eventSoursed, string corellationId);
    }
}
