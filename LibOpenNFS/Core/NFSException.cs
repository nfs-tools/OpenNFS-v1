using System;

namespace LibOpenNFS.Core
{
    public class NFSException : Exception
    {
        public NFSException()
        {
        }

        public NFSException(string message) : base(message)
        {
        }
    }
}