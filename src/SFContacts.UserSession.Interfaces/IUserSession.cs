using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SFContacts.UserSession.Interfaces {
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IUserSession : IActor {
        Task<string> GetSessionItem(string key, CancellationToken cancellationToken);
        Task SetSessionItem(string key, string value, CancellationToken cancellationToken);
        Task RemoveSessionItem(string key, CancellationToken cancellationToken);
    }
}
