using System;
using Xunit;

namespace RequestFiend.Models.Tests;

public class ExceptionModelTests {
    [Fact]
    public void Constructor() {
        Exception exception;

        try {
            throw new InvalidOperationException("Message");
        }
        catch (Exception ex) {
            exception = ex;
        }

        var subject = new ExceptionModel(exception);

        Assert.Equal(exception.GetType().FullName, subject.Type);
        Assert.Equal(exception.Message, subject.Message);
        Assert.Equal(exception.Source, subject.Source);
        Assert.Equal(exception.StackTrace, subject.StackTrace);
    }
}
