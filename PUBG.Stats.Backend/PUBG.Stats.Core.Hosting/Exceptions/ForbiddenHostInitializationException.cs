﻿using System;
using System.Runtime.Serialization;

namespace PUBG.Stats.Core.Hosting.Exceptions
{
    [Serializable]
    public class ForbiddenHostInitializationException : Exception
    {
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        public ForbiddenHostInitializationException()
        {
        }

        public ForbiddenHostInitializationException(string message)
            : base(message)
        {
        }

        public ForbiddenHostInitializationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ForbiddenHostInitializationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
