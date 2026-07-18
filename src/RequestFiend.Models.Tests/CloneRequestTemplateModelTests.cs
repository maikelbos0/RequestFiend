using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class CloneRequestTemplateModelTests {
    [Fact]
    public void Name() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new CloneRequestTemplateModel(Substitute.For<Func<CancellationToken, Task>>(), Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IMessageService>(), new(filePath), collection, request) {
            Name = { Value = "NewName" }
        };

        subject.Name.Set();

        Assert.Equal("NewName", request.Name);
    }

    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new CloneRequestTemplateModel(Substitute.For<Func<CancellationToken, Task>>(), Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IMessageService>(), new(filePath), collection, request);

        Assert.Equal(request.Name, subject.Name.Value);

        Assert.Equal([subject.Name], subject.Validatables);
    }

    [Fact]
    public async Task Clone() {
        const string filePath = @"C:\Documents\External data requests.json";

        var closeMethod = Substitute.For<System.Func<CancellationToken, Task>>();
        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new CloneRequestTemplateModel(closeMethod, requestTemplateCollectionService, messageService, new(filePath), collection, request) {
            Name = { Value = "Name" }
        };

        await subject.Clone(CancellationToken.None);

        Assert.False(subject.IsModified);

        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Is<RequestTemplateCreatedMessage>(message => message.FilePath == filePath && message.Collection == collection));
        messageService.Received(1).Send(Arg.Any<RequestTemplateAddedToCollectionMessage>(), new FileModel(filePath));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        await closeMethod.Received().Invoke(CancellationToken.None);
    }


    [Fact]
    public async Task Clone_Fails_When_Invalid() {
        const string filePath = @"C:\Documents\External data requests.json";

        var closeMethod = Substitute.For<System.Func<CancellationToken, Task>>();
        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new CloneRequestTemplateModel(closeMethod, requestTemplateCollectionService, messageService, new(filePath), collection, request) {
            Name = { Value = "" }
        };

        await subject.Clone(CancellationToken.None);

        Assert.True(subject.IsModified);

        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<RequestTemplateCreatedMessage>());
        messageService.DidNotReceive().Send(Arg.Any<RequestTemplateAddedToCollectionMessage>(), Arg.Any<FileModel>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await closeMethod.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }
}