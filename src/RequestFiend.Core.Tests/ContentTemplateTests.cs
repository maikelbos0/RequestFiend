using System;
using Xunit;

namespace RequestFiend.Core.Tests;

public class ContentTemplateTests {
    [Theory]
    [InlineData(ContentType.None, typeof(NoneContentManager))]
    [InlineData(ContentType.Text, typeof(TextContentManager))]
    [InlineData(ContentType.Json, typeof(JsonContentManager))]
    public void Manager(ContentType type, Type expectedManagerType) {
        var subject = new ContentTemplate() {
            Type = type
        };

        Assert.Equal(expectedManagerType, subject.Manager.GetType());
    }
}
