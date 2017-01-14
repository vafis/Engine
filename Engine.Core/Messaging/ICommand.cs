using System;

namespace Engine.Core.Messaging
{
    public interface ICommand
    {
        /// <summary>
        /// Command Identifier
        /// </summary>
        Guid Id { get; set; }
    }
}
