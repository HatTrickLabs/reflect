using System;

namespace HatTrick.Reflection
{
    public class NoItemExistsException : Exception
    {
        public NoItemExistsException(string message) : base(message)
        {
        }
    }
}
