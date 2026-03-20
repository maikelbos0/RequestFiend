using System;

namespace RequestFiend.Models;

public record ExceptionModel(string? Type, string Message, string? Source, string? StackTrace) {
    public ExceptionModel(Exception exception) : this(
        exception.GetType().FullName,
        exception.Message,
        exception.Source,
        exception.StackTrace
    ) { }
}
