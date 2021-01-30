using System;

namespace HaysR.FireAndForget.Exceptions
{
    public class MaxRetryHitException : Exception
    {
        public MaxRetryHitException()
        {
        }

        public MaxRetryHitException(string message)
            : base(message)
        {
        }

        public MaxRetryHitException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}