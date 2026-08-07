using System;
using Xunit;

namespace RequestFiend.Core.Tests;

public class ContentManagerProviderTests : TestsBase {
    [Theory]
    [InlineData(ContentType.None, typeof(NoneContentManager))]
    [InlineData(ContentType.Text, typeof(TextContentManager))]
    [InlineData(ContentType.Json, typeof(JsonContentManager))]
    [InlineData(ContentType.Xml, typeof(XmlContentManager))]
    [InlineData(ContentType.File, typeof(FileContentManager))]
    [InlineData(ContentType.FormData, typeof(FormDataContentManager))]
    public void Provide(ContentType contentType, Type expectedManagerType) {
        Assert.Equal(expectedManagerType, ContentManagerProvider.Provide(contentType).GetType());
    }
}
