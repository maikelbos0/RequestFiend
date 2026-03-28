using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Storage = Microsoft.Maui.Storage;

namespace RequestFiend.Models.Tests;

public class MainPageModelTests {
    [Fact]
    public void Constructor() {
        const bool showRecentCollections = true;
        var recentCollection = new List<RequestTemplateCollectionFileModel>();

        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetShowRecentCollections().Returns(showRecentCollections);
        preferencesService.GetRecentCollections().Returns(recentCollection);

        var subject = new MainPageModel(Substitute.For<IPopupService>(), messageService, preferencesService, Substitute.For<IFileSystem>());

        Assert.Equal("Home", subject.PageTitleBase);
        Assert.Equal("Home", subject.ShellItemTitleBase);
        Assert.Equal(showRecentCollections, subject.ShowRecentCollections);
        Assert.Equal(recentCollection, subject.RecentCollections);

        messageService.Received(1).Register(subject, Arg.Any<MessageHandler<MainPageModel, RecentCollectionsChangedMessage>>());
        messageService.Received(1).Register(subject, Arg.Any<MessageHandler<MainPageModel, ShowRecentCollectionsChangedMessage>>());
    }

    [Fact]
    public async Task CreateNewCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(filePath, null));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();

        var subject = new MainPageModel(popupService, messageService, preferencesService, Substitute.For<IFileSystem>());

        await subject.CreateNewCollection();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.Received(1).Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        preferencesService.Received(1).PushRecentCollection(filePath);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateNewCollection_Fails_For_Invalid_FileSaverResult() {
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new Exception()));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();

        var subject = new MainPageModel(popupService, messageService, preferencesService, Substitute.For<IFileSystem>());

        await subject.CreateNewCollection();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateNewCollection_Does_Nothing_When_Canceled() {
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new OperationCanceledException()));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();

        var subject = new MainPageModel(popupService, messageService, preferencesService, Substitute.For<IFileSystem>());

        await subject.CreateNewCollection();

        await popupService.Received(1).ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(new RequestTemplateCollection()));

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenExistingCollection();

        messageService.Received(1).Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        preferencesService.Received(1).PushRecentCollection(filePath);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection_Does_Nothing_Without_Selected_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns((Storage.FileResult?)null);
        var messageService = Substitute.For<IMessageService>();
        var recentCollectionService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);

        var subject = new MainPageModel(popupService, messageService, recentCollectionService, fileSystem);

        await subject.OpenExistingCollection();

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenExistingCollection();

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("Invalid JSON");

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenExistingCollection();

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenExistingCollection_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(filePath));
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("null");

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenExistingCollection();

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(new RequestTemplateCollection()));

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenCollection(filePath);

        messageService.Received(1).Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        preferencesService.Received(1).PushRecentCollection(filePath);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection_Fails_For_Missing_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenCollection(filePath);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection_Fails_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("Invalid JSON");

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenCollection(filePath);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task OpenCollection_Fails_For_Null_Deserialization() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var preferencesService = Substitute.For<IPreferencesService>();
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("null");

        var subject = new MainPageModel(popupService, messageService, preferencesService, fileSystem);

        await subject.OpenCollection(filePath);

        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        preferencesService.DidNotReceive().PushRecentCollection(Arg.Any<string>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }
}
