﻿using System;

namespace Morpher.WebService.V3
{
    class BadRequestException : Exception
    {
        public int Status { get; }

        public BadRequestException(int status)
        {
            Status = status;
        }
    }
}