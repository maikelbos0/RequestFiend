using RequestFiend.Core;
using RequestFiend.Models.Services;
using System;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class RequestTemplateCollectionProviderTests {
    [Fact]
    public void CreateScope_Sets_And_Clear_Data() {
        const string filePath = @"C:\Documents\External data requests.json";
        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionProvider();

        using (subject.CreateScope(filePath, collection)) {
            Assert.True(subject.Data.HasValue);
            Assert.Equal(filePath, subject.Data.Value.FilePath);
            Assert.Equal(collection, subject.Data.Value.Collection);
        }

        Assert.False(subject.Data.HasValue);
    }

    [Fact]
    public void CreateScope_Throws_For_Double_Scope() {
        const string filePath = @"C:\Documents\External data requests.json";
        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionProvider();

        using (subject.CreateScope(filePath, collection)) {
            Assert.Throws<InvalidOperationException>(() => subject.CreateScope(filePath, collection));
        }
    }
}
