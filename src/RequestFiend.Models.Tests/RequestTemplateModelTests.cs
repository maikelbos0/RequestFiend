using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
    [Theory]
    [InlineData(Core.ContentType.None, false, false)]
    [InlineData(Core.ContentType.Text, true, false)]
    [InlineData(Core.ContentType.Json, true, true)]
    public void ContentType(ContentType contentType, bool expectedUsesStringContent, bool expectedUsesJsonContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), new(filePath), collection, request) {
            ContentType = { Value = Options.ContentTypeMap[contentType] }
        };

        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesJsonContent, subject.UsesJsonContent);
    }

    [Theory]
    [InlineData(Core.ContentType.None, false, false)]
    [InlineData(Core.ContentType.Text, true, false)]
    [InlineData(Core.ContentType.Json, true, true)]
    public void Constructor(ContentType contentType, bool expectedUsesStringContent, bool expectedUsesJsonContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            Headers = {
                new() { Name = "Name", Value = "Value" }
            },
            ContentType = contentType,
            StringContent = "Content"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), new(filePath), collection, request);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name}", subject.PageTitleBase);
        Assert.Equal(request.Name, subject.ShellItemTitleBase);

        Assert.Equal(request.Name, subject.Name.Value);
        Assert.Equal(request.Method, subject.Method.Value);
        Assert.Equal(request.Url, subject.Url.Value);
        Assert.Equal(request.Headers[0].Name, subject.Headers[0].Name.Value);
        Assert.Equal(request.Headers[0].Value, subject.Headers[0].Value.Value);
        Assert.Equal(Options.ContentTypeMap[request.ContentType], subject.ContentType.Value);
        Assert.Equal(request.StringContent, subject.StringContent.Value);
        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesJsonContent, subject.UsesJsonContent);
    }

    [Fact]
    public void CreateRequest() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string name = "Name";
        const string method = "GET";
        const string url = "https://localhost";
        const string headerName = "Name";
        const string headerValue = "Value";
        const string contentType = "JSON";
        const string stringContent = "Content";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            ContentType = Core.ContentType.Text,
            StringContent = "PreviousContent"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), collection, request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.Headers[0].Value.Value = headerValue;
        subject.ContentType.Value = contentType;
        subject.StringContent.Value = stringContent;

        subject.CreateRequest();

        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.Equal("PreviousContent", request.StringContent);

        messageService.Received(1).Send(Arg.Is<CreateRequestMessage>(message
            => message.FilePath == filePath
            && message.Id == subject.Id
            && message.Collection == collection
            && message.Request != request
            && message.Request.Name == name
            && message.Request.Method == method
            && message.Request.Url == url
            && message.Request.Headers.Count == 1
            && message.Request.Headers[0].Name == headerName
            && message.Request.Headers[0].Value == headerValue
            && message.Request.ContentType == Options.ReverseContentTypeMap[contentType]
            && message.Request.StringContent == stringContent));
    }

    [Theory]
    [InlineData("", "", "", "", "")]
    [InlineData("", "GET", "https://localhost", "Name", "JSON")]
    [InlineData("Name", "", "https://localhost", "Name", "JSON")]
    [InlineData("Name", "GET", "", "Name", "JSON")]
    [InlineData("Name", "GET", "https://localhost", "", "JSON")]
    [InlineData("Name", "GET", "https://localhost", "Name", "")]
    public async Task CreateRequest_Fails_When_Invalid(string name, string method, string url, string headerName, string contentType) {
        const string filePath = @"C:\Documents\External data requests.json";
        const string stringContent = "Content";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            ContentType = Core.ContentType.Text,
            StringContent = "PreviousContent"
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), new(), request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.ContentType.Value = contentType;
        subject.StringContent.Value = stringContent;

        subject.CreateRequest();

        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.Equal("PreviousContent", request.StringContent);

        messageService.DidNotReceive().Send(Arg.Any<CreateRequestMessage>());
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string name = "Name";
        const string method = "GET";
        const string url = "https://localhost";
        const string headerName = "Name";
        const string headerValue = "Value";
        const string contentType = "JSON";
        const string stringContent = "Content";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            ContentType = Core.ContentType.Text,
            StringContent = "PreviousContent"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), collection, request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.Headers[0].Value.Value = headerValue;
        subject.ContentType.Value = contentType;
        subject.StringContent.Value = stringContent;

        await subject.Update();

        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
        Assert.Equal(headerName, request.Headers[0].Name);
        Assert.Equal(headerValue, request.Headers[0].Value);
        Assert.Equal(Options.ReverseContentTypeMap[contentType], request.ContentType);
        Assert.Equal(stringContent, request.StringContent);
        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Method.IsModified);
        Assert.False(subject.Url.IsModified);
        Assert.False(subject.Headers[0].Name.IsModified);
        Assert.False(subject.Headers[0].Value.IsModified);
        Assert.False(subject.StringContent.IsModified);

        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("", "", "", "", "")]
    [InlineData("", "GET", "https://localhost", "Name", "JSON")]
    [InlineData("Name", "", "https://localhost", "Name", "JSON")]
    [InlineData("Name", "GET", "", "Name", "JSON")]
    [InlineData("Name", "GET", "https://localhost", "", "JSON")]
    [InlineData("Name", "GET", "https://localhost", "Name", "")]
    public async Task Update_Fails_When_Invalid(string name, string method, string url, string headerName, string contentType) {
        const string filePath = @"C:\Documents\External data requests.json";
        const string stringContent = "Content";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            ContentType = Core.ContentType.Text,
            StringContent = "PreviousContent"
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), new(), request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.ContentType.Value = contentType;
        subject.StringContent.Value = stringContent;

        await subject.Update();

        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.Equal("PreviousContent", request.StringContent);

        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("{\"Object\":{\"Field\":\"Value\"}}")]
    [InlineData("{\"Array\":[0,1,2,3,4,5]}")]
    public async Task ValidateJson_When_Valid(string stringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = Core.ContentType.Json,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.ValidateJson();

        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    [InlineData("\"Field\":\"Value\"")]
    public async Task ValidateJson_When_Invalid(string stringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = Core.ContentType.Json,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.ValidateJson();

        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("{\"Object\":{\"Field\":\"Value\"}}", "{\r\n  \"Object\": {\r\n    \"Field\": \"Value\"\r\n  }\r\n}")]
    [InlineData("{\"Array\":[0,1,2,3,4,5]}", "{\r\n  \"Array\": [\r\n    0,\r\n    1,\r\n    2,\r\n    3,\r\n    4,\r\n    5\r\n  ]\r\n}")]
    public async Task FormatJson_When_Valid(string stringContent, string expectedStringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = Core.ContentType.Json,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.FormatJson();

        Assert.Equal(subject.StringContent.Value, expectedStringContent);

        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    [InlineData("\"Field\":\"Value\"")]
    public async Task FormatJson_When_Invalid(string stringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = Core.ContentType.Json,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.FormatJson();

        Assert.Equal(stringContent, subject.StringContent.Value);

        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Delete_And_Confirm() {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(true);
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, new(filePath), collection, request);

        await subject.Delete();

        Assert.Empty(collection.Requests);
        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<RequestTemplateDeletedMessage>(), subject.Id);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Delete_Without_Confirming() {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowConfirmPopup(Arg.Any<string>()).Returns(false);
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), collection, request);

        await subject.Delete();

        Assert.Equal(request, Assert.Single(collection.Requests));
        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<RequestTemplateDeletedMessage>(), Arg.Any<Guid>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
