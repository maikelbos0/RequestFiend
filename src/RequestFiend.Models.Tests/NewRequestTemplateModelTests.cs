using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NewRequestTemplateModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default"
        };

        var subject = new NewRequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), messageService, new(filePath), collection);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - New request", subject.PageTitleBase);
        Assert.Equal("New request", subject.ShellItemTitleBase);

        Assert.Equal(new RequestTemplateCollectionFileModel(filePath), subject.File);
        Assert.Equal(collection, subject.Collection);

        Assert.Equal(collection.DefaultUrl, subject.Url.Value);

        messageService.Received(1).Register(subject, filePath, Arg.Any<MessageHandler<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage>>());

        Assert.Equal([subject.Name, subject.Method, subject.Url], subject.Validatables);
    }

    [Fact]
    public async Task Create() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string name = "Name";
        const string method = "GET";
        const string url = "https://localhost";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection();

        var subject = new NewRequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), collection);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;

        await subject.Create();

        var request = Assert.Single(collection.Requests);
        Assert.NotNull(request);
        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);

        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Method.IsModified);
        Assert.False(subject.Url.IsModified);

        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("", "GET", "https://localhost")]
    [InlineData("Name", "", "https://localhost")]
    [InlineData("Name", "GET", "")]
    public async Task Create_Fails_When_Invalid(string name, string method, string url) {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection();

        var subject = new NewRequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), collection);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;

        await subject.Create();

        Assert.Empty(collection.Requests);

        Assert.Equal(name, subject.Name.Value);
        Assert.Equal(method, subject.Method.Value);
        Assert.Equal(url, subject.Url.Value);

        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ShowUrlPopup() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string expectedUrl = "https://localhost/api";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://localhost"
        };
        popupResult.Result.Returns(expectedUrl);
        popupService.ShowUrlPopup(collection, collection.DefaultUrl).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new NewRequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), collection);

        await subject.ShowUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, collection.DefaultUrl);
        Assert.Equal(expectedUrl, subject.Url.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == subject.Url));
    }

    [Fact]
    public async Task ShowUrlPopup_Without_Result() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string expectedUrl = "https://localhost";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = expectedUrl
        };
        popupResult.Result.Returns((string?)null);
        popupService.ShowUrlPopup(collection, collection.DefaultUrl).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new NewRequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), collection);

        await subject.ShowUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, collection.DefaultUrl);
        Assert.Equal(expectedUrl, subject.Url.Value);
        messageService.DidNotReceive().Send(Arg.Any<ValidatablePropertyUpdatedMessage>());
    }
}

