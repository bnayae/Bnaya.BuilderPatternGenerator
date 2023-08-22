﻿using System.Runtime.Serialization;

namespace Bnaya.BuilderPatternGenerator.SrcGen.Extensions.Exceptions;

public class SourceGeneratorException : Exception
{
    public SourceGeneratorException()
    {
    }

    public SourceGeneratorException(string message) : base(message)
    {
    }

    public SourceGeneratorException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SourceGeneratorException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
