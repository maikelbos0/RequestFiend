using RequestFiend.Core;
using RequestFiend.Models.Services;
using System;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class RequestTemplateCollectionProviderTests {
    [Fact]
    public void CreateScope_And_GetData() {
        const string filePath = @"C:\Documents\External data requests.json";
        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionProvider();

        using (subject.CreateScope(filePath, collection)) {
            var (filePathResult, collectionResult) = subject.GetData();

            Assert.Equal(filePath, filePathResult);
            Assert.Equal(collection, collectionResult);
        }
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

    [Fact]
    public void GetData_Throws_When_Outside_Scope() {
        var subject = new RequestTemplateCollectionProvider();

        Assert.Throws<InvalidOperationException>(() => subject.GetData());
    }
}
