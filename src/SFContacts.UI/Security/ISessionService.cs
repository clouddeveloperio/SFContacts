using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFContacts.UI.Security {
    public interface ISessionService {
        Task AddSessionItem<T>(string key, T value);
        Task<T> GetSessionItem<T>(string key);
    }
}
