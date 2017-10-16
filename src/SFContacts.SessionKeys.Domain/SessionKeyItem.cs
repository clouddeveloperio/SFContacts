using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFContacts.SessionKeys.Domain {
    [Serializable]
    public class SessionKeyItem {

        public SessionKeyItem(string key, string value, SessionKeyItemId id = null) {
            Id = id ?? new SessionKeyItemId();
            Key = key;
            Value = value;
        }

        public SessionKeyItemId Id { get; }
        public string Key { get; }
        public string Value { get; }

        public override string ToString() {
            return $"Session Key: {Key} with Value: {Value} at: {DateTime.UtcNow}";
        }
    }
}
