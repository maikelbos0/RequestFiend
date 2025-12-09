using CommunityToolkit.Maui.Storage;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

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

        var subject = new MainPageModel(popupService, messageService, recentCollectionService);

        await subject.CreateNewCollection();

        Assert.Same(recentCollections, subject.RecentCollections);

        await popupService.Received().ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.Received().Send(Arg.Is<OpenCollectionRequestMessage>(message => message.FilePath == filePath));
        recentCollectionService.Received().Push(filePath);
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

        var subject = new MainPageModel(popupService, messageService, recentCollectionService);

        await subject.CreateNewCollection();

        Assert.NotSame(recentCollections, subject.RecentCollections);

        await popupService.Received().ShowSaveDialog(".json", Arg.Is<MemoryStream>(stream => Encoding.Default.GetString(stream.ToArray()) == JsonSerializer.Serialize(new RequestTemplateCollection())));
        messageService.DidNotReceive().Send(Arg.Any<OpenCollectionRequestMessage>());
        recentCollectionService.DidNotReceive().Push(Arg.Any<string>());
        await popupService.Received().ShowErrorPopup(Arg.Any<string>());
    }
}
