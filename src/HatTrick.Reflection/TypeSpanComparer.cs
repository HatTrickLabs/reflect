using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatTrick.Reflection
{
    public sealed class AlternateLookupComparer : IEqualityComparer<(Type Type, string Name)>, 
                                           IAlternateEqualityComparer<AlternateLookupKey, (Type Type, string Name)>
    {
        #region create
        public (Type Type, string Name) Create(AlternateLookupKey alternate)
        {
            return (alternate.Type, alternate.Name.ToString());
        }
        #endregion

        #region equals
        public bool Equals((Type Type, string Name) x, (Type Type, string Name) y)
        {
            return x.Type == y.Type && x.Name == y.Name;
        }

        // Alternate Lookup methods using the custom ref struct
        public bool Equals(AlternateLookupKey alternate, (Type Type, string Name) other)
        {
            return alternate.Type == other.Type && alternate.Name.SequenceEqual(other.Name);
        }
        #endregion

        #region get hash code
        public int GetHashCode((Type Type, string Name) obj)
        {
            return HashCode.Combine(obj.Type, obj.Name);
        }

        public int GetHashCode(AlternateLookupKey alternate)
        {
            return HashCode.Combine(alternate.Type, string.GetHashCode(alternate.Name));
        }
        #endregion
    }
}
