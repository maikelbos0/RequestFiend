using System;

namespace RequestFiend.Models;

public record ExceptionModel(string? Type, string Message, string? Source, string? StackTrace) {
    public static ExceptionModel Create(Exception exception) 
        => new(
            exception.GetType().FullName,
            exception.Message,
            exception.Source,
            exception.StackTrace
        );
}
