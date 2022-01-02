using System;

namespace SiRandomizer.Exceptions
{
    public class SiException : Exception
    {
        public SiException() : base() {}
        public SiException(string message) : base(message) {}
        public SiException(string message, Exception inner) 
            : base(message, inner) {}
    }
}
