using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFContacts.SessionKeys.Domain {
    [DataContract]
    public class SessionKeyItemId : IFormattable, IComparable, IComparable<SessionKeyItemId>, IEquatable<SessionKeyItemId> {

        [DataMember]
        private Guid id;

        public SessionKeyItemId() {
            this.id = Guid.NewGuid();
        }

        public int CompareTo(SessionKeyItemId other) {
            return this.id.CompareTo(other.id);
        }

        public int CompareTo(object obj) {
            return this.id.CompareTo(((SessionKeyItemId)obj).id);
        }

        public bool Equals(SessionKeyItemId other) {
            return this.id.Equals(other.id);
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            return this.id.ToString(format, formatProvider);
        }

        public static bool operator ==(SessionKeyItemId item1, SessionKeyItemId item2) {
            return item1.Equals(item2);
        }

        public static bool operator !=(SessionKeyItemId item1, SessionKeyItemId item2) {
            return !item1.Equals(item2);
        }

        public override bool Equals(object obj) {
            return (obj is SessionKeyItemId) ? this.id.Equals(((SessionKeyItemId)obj).id) : false;
        }

        public override int GetHashCode() {
            return this.id.GetHashCode();
        }

        public override string ToString() {
            return this.id.ToString();
        }

        public string ToString(string format) {
            return this.id.ToString(format);
        }
    }
}
