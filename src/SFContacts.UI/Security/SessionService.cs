using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using SFContacts.UserSession.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFContacts.UI.Security {
    public class SessionService: ISessionService {
        public SessionService(IHttpContextAccessor contextAccessor) {
            HttpContext = contextAccessor?.HttpContext ?? throw new ArgumentException("contextAccessor.HttpContext is null.");
            UserSessionUriBuilder = new ServiceUriBuilder("UserSessionActorService");
        }

        public async Task AddSessionItem<T>(string key, T value) {
            try {
                await SessionActor?.SetSessionItem(key, value.ToString(), CancellationToken.None);
            }
            catch (Exception ex) {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->AddSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
        }

        public async Task<T> GetSessionItem<T>(string key) {
            try {
                var sessionItemValue = await SessionActor?.GetSessionItem(key, CancellationToken.None);
                if (string.IsNullOrEmpty(sessionItemValue)) {
                    return default(T);
                }
                return (T)Convert.ChangeType(sessionItemValue, typeof(T));
            }
            catch (Exception ex) {
                ServiceEventSource.Current.Message($"Method {nameof(SessionService)}->GetSessionItem<T>() failed with error: {ex.ToString()} at: {DateTime.UtcNow}");
            }
            return default(T);
        }

        private string GetUserSessionId() {
            var claimEnumerator = HttpContext.User.Claims.GetEnumerator();
            string userSessionId = null;
            while (claimEnumerator.MoveNext()) {
                if(string.Equals(claimEnumerator.Current.Type, ClaimTypes.NameIdentifier, StringComparison.InvariantCulture)) {
                    userSessionId = claimEnumerator.Current.Value;
                    break;
                }
            }
            return userSessionId;
        }

        IUserSession SessionActor {
            get {
                var userSessionId = GetUserSessionId();
                if (string.IsNullOrEmpty(userSessionId)) {
                    return null;
                }
                var actorId = new ActorId(userSessionId);
                return ActorProxy.Create<IUserSession>(actorId, UserSessionUriBuilder.ToUri());
            }
        }


        HttpContext HttpContext { get; }
        ServiceUriBuilder UserSessionUriBuilder { get; }
    }
}
