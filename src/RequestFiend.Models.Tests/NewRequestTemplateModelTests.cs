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

        var subject = new NewRequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), messageService, new(filePath), collection);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - New request", subject.PageTitleBase);
        Assert.Equal("New request", subject.ShellItemTitleBase);

        // TODO move the initial title update and property subscription to ConfigureState and find a way to confirm state configuration
        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - New request ▲", subject.PageTitle);
        Assert.Equal("New request ▲", subject.ShellItemTitle);

        Assert.Equal(collection.DefaultUrl, subject.Url.Value);

        messageService.Received(1).Register(subject, filePath, Arg.Any<MessageHandler<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage>>());
    }

    [Theory]
    [InlineData(false, false, "External data requests - New request", "New request")]
    [InlineData(true, false, "External data requests - New request ▲", "New request ▲")]
    [InlineData(false, true, "External data requests - New request ●", "New request ●")]
    [InlineData(true, true, "External data requests - New request ▲", "New request ▲")]
    public void UpdateTitles(bool hasError, bool isModified, string expectedPageTitle, string expectedShellItemTitle) {
        const string filePath = @"C:\Documents\External data requests.json";

        var subject = new NewRequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IMessageService>(), new(filePath), new()) {
            HasError = hasError,
            IsModified = isModified
        };

        subject.UpdateTitles();

        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Fact]
    public async Task Create() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string name = "Name";
        const string method = "GET";
        const string url = "https://url";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default"
        };

        var subject = new NewRequestTemplateModel(requestTemplateCollectionService, messageService, new(filePath), collection);

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
    [InlineData(null, null, null)]
    [InlineData(null, "GET", "https://url")]
    [InlineData("Name", null, "https://url")]
    [InlineData("Name", "GET", null)]
    public async Task Create_Fails_When_Invalid(string? name, string? method, string? url) {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var collection = new RequestTemplateCollection() {
            DefaultUrl = "https://default"
        };

        var subject = new NewRequestTemplateModel(requestTemplateCollectionService, messageService, new(filePath), collection);

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
}

