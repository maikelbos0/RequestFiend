using CommunityToolkit.Maui.Storage;
using NSubstitute;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class LogModelTests {
    [Fact]
    public void Constructor() {
        var subject = new LogModel(Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), 1);

        Assert.Equal("Exchange log", subject.PageTitleBase);
        Assert.Equal("Exchange log", subject.ShellItemTitleBase);

        Assert.Equal([], subject.Validatables);
    }

    [Fact]
    public async Task Add_And_StartUpdating() {
        var subject = new LogModel(Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), 1);
        var cancellationTokenSource = new CancellationTokenSource();
        var updatingTask = subject.StartUpdating(cancellationTokenSource.Token);

        subject.Add($"Test 1{Environment.NewLine}");
        subject.Add($"Test 2{Environment.NewLine}");

        await Task.Delay(10, TestContext.Current.CancellationToken);

        cancellationTokenSource.Cancel();
        await updatingTask;

        Assert.Equal($"Test 1{Environment.NewLine}Test 2{Environment.NewLine}", subject.LogEvents);
    }

    [Fact]
    public async Task Clear_And_StartUpdating() {
        var subject = new LogModel(Substitute.For<IMessageService>(), Substitute.For<IPopupService>(), 1);
        var cancellationTokenSource = new CancellationTokenSource();
        var updatingTask = subject.StartUpdating(cancellationTokenSource.Token);

        subject.Add($"Test 1{Environment.NewLine}");
        subject.Add($"Test 2{Environment.NewLine}");

        subject.Clear();

        cancellationTokenSource.Cancel();
        await updatingTask;

        await Task.Delay(10, TestContext.Current.CancellationToken);

        Assert.Empty(subject.LogEvents);
    }

    [Fact]
    public async Task Save() {
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, null));

        var subject = new LogModel(messageService, popupService, 1) {
            LogEvents = $"Test 1{Environment.NewLine}Test 2{Environment.NewLine}"
        };

        await subject.Save();

        await popupService.Received(1).ShowSaveDialog(".log", Arg.Is<MemoryStream>(stream => stream.ToArray().SequenceEqual(Encoding.UTF8.GetBytes(subject.LogEvents))));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task Save_Fails_For_Invalid_FileSaverResult() {
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new Exception()));

        var subject = new LogModel(messageService, popupService, 1) {
            LogEvents = $"Test 1{Environment.NewLine}Test 2{Environment.NewLine}"
        };

        await subject.Save();

        await popupService.Received(1).ShowSaveDialog(".log", Arg.Is<MemoryStream>(stream => stream.ToArray().SequenceEqual(Encoding.UTF8.GetBytes(subject.LogEvents))));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task Save_Does_Nothing_When_Canceled() {
        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new OperationCanceledException()));

        var subject = new LogModel(messageService, popupService, 1) {
            LogEvents = $"Test 1{Environment.NewLine}Test 2{Environment.NewLine}"
        };

        await subject.Save();

        await popupService.Received(1).ShowSaveDialog(".log", Arg.Is<MemoryStream>(stream => stream.ToArray().SequenceEqual(Encoding.UTF8.GetBytes(subject.LogEvents))));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }
}
