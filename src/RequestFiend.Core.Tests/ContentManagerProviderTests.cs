using NSubstitute;
using System;
using System.IO.Abstractions;
using Xunit;

namespace RequestFiend.Core.Tests;

public class ContentManagerProviderTests {
    [Theory]
    [InlineData(ContentType.None, typeof(NoneContentManager))]
    [InlineData(ContentType.Text, typeof(TextContentManager))]
    [InlineData(ContentType.Json, typeof(JsonContentManager))]
    [InlineData(ContentType.Xml, typeof(XmlContentManager))]
    [InlineData(ContentType.File, typeof(FileContentManager))]
    [InlineData(ContentType.FormData, typeof(FormDataContentManager))]
    public void Provide(ContentType contentType, Type expectedManagerType) {
        ContentManagerProvider.Initialize(Substitute.For<IFileSystem>());
        
        Assert.Equal(expectedManagerType, ContentManagerProvider.Provide(contentType).GetType());
    }
}
