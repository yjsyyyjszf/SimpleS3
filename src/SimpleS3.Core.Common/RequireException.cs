﻿using System;

namespace Genbox.SimpleS3.Core.Common
{
    public class RequireException : Exception
    {
        public RequireException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RequireException(string message) : base(message)
        {
        }
    }
}