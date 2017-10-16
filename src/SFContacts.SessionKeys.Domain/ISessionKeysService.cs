using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFContacts.SessionKeys.Domain {
    public interface ISessionKeysService : IService {
        Task<long> GetKeyCountAsync(CancellationToken cancellationToken);
        Task AddKey(SessionKeyItem keyItem);
        Task<IEnumerable<SessionKeyItem>> GetKeys(CancellationToken cancellationToken);
    }
}
