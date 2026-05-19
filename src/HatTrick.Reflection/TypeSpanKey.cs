using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatTrick.Reflection
{
    public readonly ref struct AlternateLookupKey
    {
        #region interface
        public readonly Type Type;
        public readonly ReadOnlySpan<char> Name;
        #endregion

        #region ctor
        public AlternateLookupKey(Type type, ReadOnlySpan<char> name)
        {
            Type = type;
            Name = name;
        }
        #endregion
    }
}
