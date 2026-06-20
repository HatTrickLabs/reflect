using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatTrick.Reflection
{
    public class RecursionStackDepthException : InvalidOperationException
    {
        #region ctors
        public RecursionStackDepthException(string message) : base(message)
        { }
        #endregion
    }
}
