using Microsoft.Maui.Controls;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelTests : TestsBase {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection() {
            Requests = {
                new() { Name = "Request", Method = "GET", Url = "https://localhost" }
            }
        };

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            Substitute.For<IEnvironmentService>(),
            Substitute.For<ISecretEncryptor>(),
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
            Substitute.For<IEnvironmentService>(),
            Substitute.For<ISecretEncryptor>(),
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

    [Fact]
    public void RemoveRequest() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection() {
            Requests = {
                new() { Name = "Request", Method = "GET", Url = "https://localhost" }
            }
        };

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            Substitute.For<IEnvironmentService>(),
            Substitute.For<ISecretEncryptor>(),
            new(filePath),
            collection
        );

        subject.RemoveRequest(subject.Requests.Single());

        Assert.Empty(subject.Requests);

        Assert.Equal([subject.Settings, subject.NewRequest], subject.Validatables);
    }

    [Fact]
    public void SynchronizeRequests() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection() {
            Requests = {
                new() { Name = "First", Method = "GET", Url = "https://localhost" },
                new() { Name = "Second", Method = "GET", Url = "https://localhost" },
                new() { Name = "Third", Method = "GET", Url = "https://localhost" }
            }
        };

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            Substitute.For<IPreferencesService>(),
            Substitute.For<IEnvironmentService>(),
            Substitute.For<ISecretEncryptor>(),
            new(filePath),
            collection
        );

        var collectionItem = new Tab() { Items = { new ContentPage() } };

        var request1 = subject.Requests.ElementAt(0);
        var request1Item = new Tab() { Items = { new ContentPage() { BindingContext = request1 } } };
        var request1ExchangeItem = new Tab() { Items = { new ContentPage() } };
        
        var request2 = subject.Requests.ElementAt(1);
        var request2Item = new Tab() { Items = { new ContentPage() { BindingContext = request2 } } };
        
        var request3 = subject.Requests.ElementAt(2);
        var request3Item = new Tab() { Items = { new ContentPage() { BindingContext = request3 } } };
        var request3ExchangeItem = new Tab() { Items = { new ContentPage() } };

        var collectionItems = new List<ShellSection>() {
            collectionItem,
            request1Item,
            request1ExchangeItem,
            request2Item,
            request3Item,
            request3ExchangeItem
        };

        collection.Requests.Reverse();

        subject.SynchronizeRequests(collectionItems);

        Assert.Equal([request3, request2, request1], subject.Requests);
        Assert.Equal([collectionItem, request3Item, request3ExchangeItem, request2Item, request1Item, request1ExchangeItem], collectionItems);
    }
}
