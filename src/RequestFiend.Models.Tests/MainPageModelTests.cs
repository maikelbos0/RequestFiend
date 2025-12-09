using CommunityToolkit.Maui.Storage;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Storage = Microsoft.Maui.Storage;

namespace RequestFiend.Models.Tests;

public class MainPageModelTests {
    [Fact]
    public async Task CreateNewCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(filePath, null));
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, Substitute.For<IFileSystem>());

        await subject.CreateNewCollection();

        Assert.Same(recentCollections, subject.RecentCollections);

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.Received(1).Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        recentCollectionService.Received(1).Push(filePath);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateNewCollection_Fails_For_Invalid_FileSaverResult() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new Exception()));
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, Substitute.For<IFileSystem>());

        await subject.CreateNewCollection();

        Assert.NotSame(recentCollections, subject.RecentCollections);

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }



    [Fact]
    public async Task OpenExistingCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath).Returns(JsonSerializer.Serialize(new RequestTemplateCollection()));

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenExistingCollection();

        Assert.Same(recentCollections, subject.RecentCollections);

        messageService.Received(1).Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        recentCollectionService.Received(1).Push(filePath);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }
    
    [Fact]
    public async Task OpenExistingCollection_Does_Nothing_Without_Selected_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns((Storage.FileResult?)null);
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenExistingCollection();

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }
    
    [Fact]
    public async Task OpenExistingCollection_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenExistingCollection();

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath).Returns("Invalid JSON");

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenExistingCollection();

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath).Returns("null");

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenExistingCollection();

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath).Returns(JsonSerializer.Serialize(new RequestTemplateCollection()));

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenCollection(filePath);

        Assert.Same(recentCollections, subject.RecentCollections);

        messageService.Received(1).Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        recentCollectionService.Received(1).Push(filePath);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenCollection(filePath);

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath).Returns("Invalid JSON");

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenCollection(filePath);

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IRecentCollectionService>();
        var recentCollections = new List<RecentCollectionModel>();
        recentCollectionService.Push(filePath).Returns(recentCollections);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath).Returns("null");

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenCollection(filePath);

        Assert.NotSame(recentCollections, subject.RecentCollections);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }
}
