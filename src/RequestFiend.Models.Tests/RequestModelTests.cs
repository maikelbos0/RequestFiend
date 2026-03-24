using CommunityToolkit.Maui.Storage;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), Substitute.For<IPopupService>(), new(filePath), collection, request);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Exchange", subject.PageTitleBase);
        Assert.Equal($"{request.Name} - Exchange", subject.ShellItemTitleBase);
    }

    [Fact]
    public async Task Execute() {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestHandler = Substitute.For<IRequestHandler>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();

        var subject = new RequestModel(Substitute.For<IMessageService>(), requestHandler, Substitute.For<IPopupService>(), new(filePath), collection, request);
        subject.PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
                case nameof(RequestModel.PageTitleBase):
                    pageTitleBaseValues.Add(subject.PageTitleBase);
                    break;
                case nameof(RequestModel.ShellItemTitleBase):
                    shellItemTitleBaseValues.Add(subject.ShellItemTitleBase);
                    break;
                case nameof(RequestModel.IsExecuting):
                    isExecutingValues.Add(subject.IsExecuting);
                    break;
                default:
                    break;
            }
        };

        await subject.Execute();

        _ = requestHandler.Received().Execute(request, collection, subject, Arg.Any<CancellationToken>());
        Assert.Equal([$"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Executing request...", $"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name} - Exchange"], pageTitleBaseValues);
        Assert.Equal([$"{request.Name} - Executing request...", $"{request.Name} - Exchange"], shellItemTitleBaseValues);
        Assert.Equal([true, false], isExecutingValues);
    }

    [Theory]
    [InlineData(HttpContentType.Text, "application/json", ".json")]
    [InlineData(HttpContentType.Image, "image/gif", ".gif")]
    [InlineData(HttpContentType.Text, "application/unknown", ".txt")]
    [InlineData(HttpContentType.Unknown, "application/unknown", ".bin")]
    [InlineData(HttpContentType.Text, null, ".txt")]
    [InlineData(HttpContentType.Unknown, null, ".bin")]
    public async Task SaveResponseContent(HttpContentType contentType, string? mediaType, string expectedExtension) {
        const string filePath = @"C:\Documents\External data requests.json";

        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(filePath, null));
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();

        var subject = new RequestModel(messageService, Substitute.For<IRequestHandler>(), popupService, new(filePath), collection, request) {
            Response = new("200 OK", [], new(contentType, mediaType, [0, 1, 2, 3], null))
        };

        await subject.SaveResponseContent();

        await popupService.Received(1).ShowSaveDialog(expectedExtension, Arg.Is<MemoryStream>(stream => stream.ToArray().SequenceEqual(subject.Response.Content.BinaryContent)));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task SaveResponseContent_Fails_For_Invalid_FileSaverResult() {
        const string filePath = @"C:\Documents\External data requests.json";

        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowSaveDialog(Arg.Any<string>(), Arg.Any<Stream>()).Returns(new FileSaverResult(null, new Exception()));
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();

        var subject = new RequestModel(messageService, Substitute.For<IRequestHandler>(), popupService, new(filePath), collection, request) {
            Response = new("200 OK", [], new(HttpContentType.Text, "text/plain", [0, 1, 2, 3], null))
        };

        await subject.SaveResponseContent();

        await popupService.Received(1).ShowSaveDialog(Arg.Any<string>(), Arg.Is<MemoryStream>(stream => stream.ToArray().SequenceEqual(subject.Response.Content.BinaryContent)));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task SaveResponseContent_Does_Nothing_Without_BinaryContent() {
        const string filePath = @"C:\Documents\External data requests.json";

        var messageService = Substitute.For<IMessageService>();
        var popupService = Substitute.For<IPopupService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();

        var subject = new RequestModel(messageService, Substitute.For<IRequestHandler>(), popupService, new(filePath), collection, request) {
            Response = new("200 OK", [], new(HttpContentType.None, null, null, null))
        };

        await subject.SaveResponseContent();

        await popupService.DidNotReceive().ShowSaveDialog(Arg.Any<string>(), Arg.Is<MemoryStream>(stream => stream.ToArray().SequenceEqual(subject.Response.Content.BinaryContent)));
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());

    }

    /*
    [RelayCommand]
    public async Task SaveResponseContent() {
        if (Response?.Content?.BinaryContent == null) {
            return;
        }

        var extension = GetExtension();
        var saveResult = await popupService.ShowSaveDialog(GetExtension(), new MemoryStream(Response.Content.BinaryContent));

        if (saveResult.IsSuccessful) {
            messageService.Send(new SuccessMessage("Response content has been saved"));
        }
        else if (saveResult.Exception != null) {
            await popupService.ShowErrorPopup($"Failed to save response content: {saveResult.Exception.Message}");
        }

        string GetExtension() {
            if (Response.Content.MediaType != null) {
                var extensions = MimeUtility.GetExtensions(Response.Content.MediaType);

                if (extensions != null) {
                    return extensions[0];
                }
            }

            if (Response.Content.Type == HttpContentType.Text) {
                return ".txt";
            }

            return ".bin";
        }
    }*/
    [Fact]
    public void Close() {
        const string filePath = @"C:\Documents\External data requests.json";

        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestModel(messageService, Substitute.For<IRequestHandler>(), Substitute.For<IPopupService>(), new(filePath), collection, request);

        subject.Close();

        messageService.Received().Send(Arg.Any<CloseRequestMessage>(), subject.Id);
    }

    [Fact]
    public async Task OnRequestCreated() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();
        var expectedRequest = new HttpRequestMessage();

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), Substitute.For<IPopupService>(), new(filePath), collection, request);

        await subject.OnRequestCreated(expectedRequest);

        Assert.NotNull(subject.Request);
    }

    [Fact]
    public async Task OnResponseReceived() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();
        var expectedResponse = new HttpResponseMessage();

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), Substitute.For<IPopupService>(), new(filePath), collection, request);

        await subject.OnResponseReceived(expectedResponse);

        Assert.NotNull(subject.Response);
    }

    [Fact]
    public async Task OnExceptionCaught() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };
        var isExecutingValues = new List<bool>();
        var pageTitleBaseValues = new List<string>();
        var shellItemTitleBaseValues = new List<string>();
        var expectedException = new Exception();

        var subject = new RequestModel(Substitute.For<IMessageService>(), Substitute.For<IRequestHandler>(), Substitute.For<IPopupService>(), new(filePath), collection, request);

        await subject.OnExceptionCaught(expectedException);

        Assert.NotNull(subject.Exception);
    }
}
