using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.IO;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection() {
            Requests = [
                new() { Name = "Request", Method = "GET", Url = "https://localhost" },
            ]
        };

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        );

        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), subject.PageTitleBase);
        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), subject.ShellItemTitleBase);

        Assert.Same(collection, subject.Settings.Collection);
        Assert.Equal(new(filePath), subject.Settings.File);

        Assert.Same(collection, subject.NewRequest.Collection);
        Assert.Equal(new(filePath), subject.NewRequest.File);

        var requestModel = Assert.Single(subject.Requests);
        Assert.Same(collection, requestModel.Collection);
        Assert.Same(collection.Requests[0], requestModel.Request);
        Assert.Equal(new(filePath), requestModel.File);

        Assert.Equal([subject.Settings, subject.NewRequest, requestModel], subject.Validatables);
    }

    [Fact]
    public void AddRequest() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            new(filePath),
            collection
        );

        var request = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost"
        };

        var requestModel = subject.AddRequest(request);

        Assert.Same(requestModel, Assert.Single(subject.Requests));
        Assert.Same(collection, requestModel.Collection);
        Assert.Same(request, requestModel.Request);
        Assert.Equal(new(filePath), requestModel.File);

        Assert.Equal([subject.Settings, subject.NewRequest, requestModel], subject.Validatables);
    }
}
