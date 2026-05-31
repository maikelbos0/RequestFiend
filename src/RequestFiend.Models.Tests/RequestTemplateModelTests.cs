using CommunityToolkit.Maui.Core;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Storage = Microsoft.Maui.Storage;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
    [Theory]
    [InlineData(Core.ContentType.None, false, false, false, false, false)]
    [InlineData(Core.ContentType.Text, true, false, true, true, false)]
    [InlineData(Core.ContentType.Json, true, true, false, true, false)]
    [InlineData(Core.ContentType.Xml, true, true, false, true, false)]
    [InlineData(Core.ContentType.File, true, false, true, false, true)]
    public void ContentType(
        ContentType contentType,
        bool expectedUsesContent,
        bool expectedUsesStructuredContent,
        bool expectedUsesUnstructuredContent,
        bool expectedUsesStringContent,
        bool expectedUsesFileContent
    ) {
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

        Assert.Equal(expectedUsesContent, subject.UsesContent);
        Assert.Equal(expectedUsesStructuredContent, subject.UsesStructuredContent);
        Assert.Equal(expectedUsesUnstructuredContent, subject.UsesUnstructuredContent);
        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesFileContent, subject.UsesFileContent);
    }

    [Theory]
    [InlineData(Core.ContentType.None, false, false, false, false, false)]
    [InlineData(Core.ContentType.Text, true, false, true, true, false)]
    [InlineData(Core.ContentType.Json, true, true, false, true, false)]
    [InlineData(Core.ContentType.Xml, true, true, false, true, false)]
    [InlineData(Core.ContentType.File, true, false, true, false, true)]
    public void Constructor(
        ContentType contentType,
        bool expectedUsesContent,
        bool expectedUsesStructuredContent,
        bool expectedUsesUnstructuredContent,
        bool expectedUsesStringContent,
        bool expectedUsesFileContent
    ) {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            Headers = {
                new() { Name = "Name", Value = "Value" }
            },
            ContentType = contentType,
            HasManualContentTypeHeader = true,
            StringContent = "StringContent",
            FileContent = "FileContent",
            FormFieldContent = {
                new() { Name = "Name", Value = "Value" }
            },
            PreExchangeScript = { Code = "PreExchangeScript" },
            PostExchangeScript = { Code = "PostExchangeScript" },
            OnExceptionScript = { Code = "OnExceptionScript" }
        };
        var collection = new RequestTemplateCollection() {
            Requests = [request]
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), new(filePath), collection, request);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name}", subject.PageTitleBase);
        Assert.Equal(request.Name, subject.ShellItemTitleBase);

        Assert.Equal(new RequestTemplateCollectionFileModel(filePath), subject.File);
        Assert.Equal(collection, subject.Collection);
        Assert.Equal(request, subject.Request);

        Assert.Equal(request.Name, subject.Name.Value);
        Assert.Equal(request.Method, subject.Method.Value);
        Assert.Equal(request.Url, subject.Url.Value);
        Assert.Equal(request.Headers.Count, subject.Headers.Count);
        Assert.Equal(Options.ContentTypeMap[request.ContentType], subject.ContentType.Value);
        Assert.Equal(request.HasManualContentTypeHeader, subject.HasManualContentTypeHeader.Value);
        Assert.Equal(request.StringContent, subject.StringContent.Value);
        Assert.Equal(request.FileContent, subject.FileContent.Value);
        Assert.Equal(request.FormFieldContent.Count, subject.FormFieldContent.Count);
        Assert.Equal(request.PreExchangeScript.Code, subject.PreExchangeScript.Code.Value);
        Assert.Equal(request.PostExchangeScript.Code, subject.PostExchangeScript.Code.Value);
        Assert.Equal(request.OnExceptionScript.Code, subject.OnExceptionScript.Code.Value);

        Assert.Equal(expectedUsesContent, subject.UsesContent);
        Assert.Equal(expectedUsesStructuredContent, subject.UsesStructuredContent);
        Assert.Equal(expectedUsesUnstructuredContent, subject.UsesUnstructuredContent);
        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesFileContent, subject.UsesFileContent);

        Assert.Equal([
            subject.Name,
            subject.Method,
            subject.Url,
            subject.Headers,
            subject.ContentType,
            subject.HasManualContentTypeHeader,
            subject.StringContent,
            subject.FileContent,
            subject.FormFieldContent,
            subject.PreExchangeScript,
            subject.PostExchangeScript,
            subject.OnExceptionScript
        ], subject.Validatables);
    }

    [Fact]
    public void CreateRequest() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string name = "Name";
        const string method = "GET";
        const string url = "https://localhost";
        const string headerName = "Name";
        const string headerValue = "Value";
        const string contentType = "File";
        const bool hasManualContentTypeHeader = false;
        const string stringContent = "StringContent";
        const string fileContent = "./Data.json";
        const string formFieldName = "Name";
        const string formFieldValue = "Value";
        const string preExchangeScript = "PreExchangeScript";
        const string postExchangeScript = "PostExchangeScript";
        const string onExceptionScript = "OnExceptionScript";

        File.WriteAllBytes(fileContent, [70, 111, 111]);

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
            HasManualContentTypeHeader = true,
            StringContent = "PreviousStringContent",
            FileContent = "PreviousFileContent",
            FormFieldContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            PreExchangeScript = { Code = "PreviousPreExchangeScript" },
            PostExchangeScript = { Code = "PreviousPostExchangeScript" },
            OnExceptionScript = { Code = "PreviousOnExceptionScript" }
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
        subject.HasManualContentTypeHeader.Value = hasManualContentTypeHeader;
        subject.StringContent.Value = stringContent;
        subject.FileContent.Value = fileContent;
        subject.FormFieldContent[0].Name.Value = formFieldName;
        subject.FormFieldContent[0].Value.Value = formFieldValue;
        subject.PreExchangeScript.Code.Value = preExchangeScript;
        subject.PostExchangeScript.Code.Value = postExchangeScript;
        subject.OnExceptionScript.Code.Value = onExceptionScript;

        subject.CreateRequest();

        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.True(request.HasManualContentTypeHeader);
        Assert.Equal("PreviousStringContent", request.StringContent);
        Assert.Equal("PreviousFileContent", request.FileContent);
        Assert.Equal("PreviousName", request.FormFieldContent[0].Name);
        Assert.Equal("PreviousValue", request.FormFieldContent[0].Value);
        Assert.Equal("PreviousPreExchangeScript", request.PreExchangeScript.Code);
        Assert.Equal("PreviousPostExchangeScript", request.PostExchangeScript.Code);
        Assert.Equal("PreviousOnExceptionScript", request.OnExceptionScript.Code);

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
            && message.Request.HasManualContentTypeHeader == hasManualContentTypeHeader
            && message.Request.StringContent == stringContent
            && message.Request.FileContent == fileContent
            && message.Request.FormFieldContent[0].Name == formFieldName
            && message.Request.FormFieldContent[0].Value == formFieldValue
            && message.Request.PreExchangeScript.Code == preExchangeScript
            && message.Request.PostExchangeScript.Code == postExchangeScript
            && message.Request.OnExceptionScript.Code == onExceptionScript));
    }

    [Theory]
    [InlineData("", "", "", "", "", "", "")]
    [InlineData("", "POST", "https://localhost", "Name", "JSON", "", "Name")]
    [InlineData("Name", "", "https://localhost", "Name", "JSON", "", "Name")]
    [InlineData("Name", "POST", "", "Name", "JSON", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "", "JSON", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "Name", "", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "Name", "File", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "Name", "JSON", "", "")]
    public async Task CreateRequest_Fails_When_Invalid(string name, string method, string url, string headerName, string contentType, string fileContent, string formFieldName) {
        const string filePath = @"C:\Documents\External data requests.json";
        const string stringContent = "StringContent";
        const bool hasManualContentTypeHeader = false;
        const string preExchangeScript = "PreExchangeScript";
        const string postExchangeScript = "PostExchangeScript";
        const string onExceptionScript = "OnExceptionScript";

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
            HasManualContentTypeHeader = true,
            StringContent = "PreviousStringContent",
            FileContent = "PreviousFileContent",
            FormFieldContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            PreExchangeScript = { Code = "PreviousPreExchangeScript" },
            PostExchangeScript = { Code = "PreviousPostExchangeScript" },
            OnExceptionScript = { Code = "PreviousOnExceptionScript" }
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), new(), request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.ContentType.Value = contentType;
        subject.HasManualContentTypeHeader.Value = hasManualContentTypeHeader;
        subject.StringContent.Value = stringContent;
        subject.FileContent.Value = fileContent;
        subject.FormFieldContent[0].Name.Value = formFieldName;
        subject.PreExchangeScript.Code.Value = preExchangeScript;
        subject.PostExchangeScript.Code.Value = postExchangeScript;
        subject.OnExceptionScript.Code.Value = onExceptionScript;

        subject.CreateRequest();

        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.True(request.HasManualContentTypeHeader);
        Assert.Equal("PreviousStringContent", request.StringContent);
        Assert.Equal("PreviousFileContent", request.FileContent);
        Assert.Equal("PreviousName", request.FormFieldContent[0].Name);
        Assert.Equal("PreviousValue", request.FormFieldContent[0].Value);
        Assert.Equal("PreviousPreExchangeScript", request.PreExchangeScript.Code);
        Assert.Equal("PreviousPostExchangeScript", request.PostExchangeScript.Code);
        Assert.Equal("PreviousOnExceptionScript", request.OnExceptionScript.Code);

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
        const string contentType = "File";
        const bool hasManualContentTypeHeader = false;
        const string stringContent = "StringContent";
        const string fileContent = "./Data.json";
        const string formFieldName = "Name";
        const string formFieldValue = "Value";
        const string preExchangeScript = "PreExchangeScript";
        const string postExchangeScript = "PostExchangeScript";
        const string onExceptionScript = "OnExceptionScript";

        File.WriteAllBytes(fileContent, [70, 111, 111]);

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
            HasManualContentTypeHeader = true,
            StringContent = "PreviousStringContent",
            FileContent = "PreviousFileContent",
            FormFieldContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            PreExchangeScript = { Code = "PreviousPreExchangeScript" },
            PostExchangeScript = { Code = "PreviousPostExchangeScript" },
            OnExceptionScript = { Code = "PreviousOnExceptionScript" }
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
        subject.HasManualContentTypeHeader.Value = hasManualContentTypeHeader;
        subject.StringContent.Value = stringContent;
        subject.FileContent.Value = fileContent;
        subject.FormFieldContent[0].Name.Value = formFieldName;
        subject.FormFieldContent[0].Value.Value = formFieldValue;
        subject.PreExchangeScript.Code.Value = preExchangeScript;
        subject.PostExchangeScript.Code.Value = postExchangeScript;
        subject.OnExceptionScript.Code.Value = onExceptionScript;

        await subject.Update();

        Assert.Equal(name, request.Name);
        Assert.Equal(method, request.Method);
        Assert.Equal(url, request.Url);
        Assert.Equal(headerName, request.Headers[0].Name);
        Assert.Equal(headerValue, request.Headers[0].Value);
        Assert.Equal(Options.ReverseContentTypeMap[contentType], request.ContentType);
        Assert.Equal(hasManualContentTypeHeader, request.HasManualContentTypeHeader);
        Assert.Equal(stringContent, request.StringContent);
        Assert.Equal(fileContent, request.FileContent);
        Assert.Equal(formFieldName, request.FormFieldContent[0].Name);
        Assert.Equal(formFieldValue, request.FormFieldContent[0].Value);
        Assert.Equal(preExchangeScript, request.PreExchangeScript.Code);
        Assert.Equal(postExchangeScript, request.PostExchangeScript.Code);
        Assert.Equal(onExceptionScript, request.OnExceptionScript.Code);

        Assert.False(subject.Name.IsModified);
        Assert.False(subject.Method.IsModified);
        Assert.False(subject.Url.IsModified);
        Assert.False(subject.Headers[0].Name.IsModified);
        Assert.False(subject.Headers[0].Value.IsModified);
        Assert.False(subject.ContentType.IsModified);
        Assert.False(subject.HasManualContentTypeHeader.IsModified);
        Assert.False(subject.StringContent.IsModified);
        Assert.False(subject.FileContent.IsModified);
        Assert.False(subject.FormFieldContent[0].Name.IsModified);
        Assert.False(subject.FormFieldContent[0].Value.IsModified);
        Assert.False(subject.PreExchangeScript.IsModified);
        Assert.False(subject.PostExchangeScript.IsModified);
        Assert.False(subject.OnExceptionScript.IsModified);

        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("", "", "", "", "", "", "")]
    [InlineData("", "POST", "https://localhost", "Name", "JSON", "", "Name")]
    [InlineData("Name", "", "https://localhost", "Name", "JSON", "", "Name")]
    [InlineData("Name", "POST", "", "Name", "JSON", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "", "JSON", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "Name", "", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "Name", "File", "", "Name")]
    [InlineData("Name", "POST", "https://localhost", "Name", "JSON", "", "")]
    public async Task Update_Fails_When_Invalid(string name, string method, string url, string headerName, string contentType, string fileContent, string formFieldName) {
        const string filePath = @"C:\Documents\External data requests.json";
        const bool hasManualContentTypeHeader = false;
        const string stringContent = "Content";
        const string preExchangeScript = "PreExchangeScript";
        const string postExchangeScript = "PostExchangeScript";
        const string onExceptionScript = "OnExceptionScript";

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
            HasManualContentTypeHeader = true,
            StringContent = "PreviousStringContent",
            FileContent = "PreviousFileContent",
            FormFieldContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            PreExchangeScript = { Code = "PreviousPreExchangeScript" },
            PostExchangeScript = { Code = "PreviousPostExchangeScript" },
            OnExceptionScript = { Code = "PreviousOnExceptionScript" }
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, new(filePath), new(), request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.ContentType.Value = contentType;
        subject.HasManualContentTypeHeader.Value = hasManualContentTypeHeader;
        subject.StringContent.Value = stringContent;
        subject.FileContent.Value = fileContent;
        subject.FormFieldContent[0].Name.Value = formFieldName;
        subject.PreExchangeScript.Code.Value = preExchangeScript;
        subject.PostExchangeScript.Code.Value = postExchangeScript;
        subject.OnExceptionScript.Code.Value = onExceptionScript;

        await subject.Update();

        Assert.Equal("Old", request.Name);
        Assert.Equal("POST", request.Method);
        Assert.Equal("https://previous", request.Url);
        Assert.Equal("PreviousName", request.Headers[0].Name);
        Assert.Equal("PreviousValue", request.Headers[0].Value);
        Assert.Equal(Core.ContentType.Text, request.ContentType);
        Assert.True(request.HasManualContentTypeHeader);
        Assert.Equal("PreviousStringContent", request.StringContent);
        Assert.Equal("PreviousFileContent", request.FileContent);
        Assert.Equal("PreviousName", request.FormFieldContent[0].Name);
        Assert.Equal("PreviousValue", request.FormFieldContent[0].Value);
        Assert.Equal("PreviousPreExchangeScript", request.PreExchangeScript.Code);
        Assert.Equal("PreviousPostExchangeScript", request.PostExchangeScript.Code);
        Assert.Equal("PreviousOnExceptionScript", request.OnExceptionScript.Code);

        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ShowUrlPopup() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string expectedUrl = "https://localhost/api";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        popupResult.Result.Returns(expectedUrl);
        popupService.ShowUrlPopup(collection, request.Url).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), collection, request);

        await subject.ShowUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, request.Url);
        Assert.Equal(expectedUrl, subject.Url.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == subject.Url));
    }

    [Fact]
    public async Task ShowUrlPopup_Without_Result() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string expectedUrl = "https://localhost";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = expectedUrl
        };
        popupResult.Result.Returns((string?)null);
        popupService.ShowUrlPopup(collection, request.Url).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), collection, request);

        await subject.ShowUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, request.Url);
        Assert.Equal(expectedUrl, subject.Url.Value);
        messageService.DidNotReceive().Send(Arg.Any<ValidatablePropertyUpdatedMessage>());
    }

    [Theory]
    [InlineData(Core.ContentType.Json, "{\"Object\":{\"Field\":\"Value\"}}")]
    [InlineData(Core.ContentType.Json, "{\"Array\":[0,1,2,3,4,5]}")]
    [InlineData(Core.ContentType.Xml, "<data lang=\"nl\"><value>1</value><value>2</value><value>3</value></data>")]
    public async Task ValidateStructuredText_When_Valid(ContentType contentType, string stringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = contentType,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.ValidateStructuredText();

        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData(Core.ContentType.Json, "")]
    [InlineData(Core.ContentType.Json, "Text")]
    [InlineData(Core.ContentType.Json, "\"Field\":\"Value\"")]
    [InlineData(Core.ContentType.Xml, "")]
    [InlineData(Core.ContentType.Xml, "Text")]
    [InlineData(Core.ContentType.Xml, "<data></value>")]
    public async Task ValidateStructuredText_When_Invalid(ContentType contentType, string stringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = contentType,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.ValidateStructuredText();

        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData(Core.ContentType.Json, "{\"Object\":{\"Field\":\"Value\"}}", "{\r\n  \"Object\": {\r\n    \"Field\": \"Value\"\r\n  }\r\n}")]
    [InlineData(Core.ContentType.Json, "{\"Array\":[0,1,2,3,4,5]}", "{\r\n  \"Array\": [\r\n    0,\r\n    1,\r\n    2,\r\n    3,\r\n    4,\r\n    5\r\n  ]\r\n}")]
    [InlineData(Core.ContentType.Xml, "<data lang=\"nl\"><value>1</value><value>2</value><value>3</value></data>", "<data lang=\"nl\">\r\n  <value>1</value>\r\n  <value>2</value>\r\n  <value>3</value>\r\n</data>")]
    public async Task FormatStructuredText_When_Valid(ContentType contentType, string stringContent, string expectedStringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = contentType,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.FormatStructuredText();

        Assert.Equal(expectedStringContent, subject.StringContent.Value);

        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData(Core.ContentType.Json, "")]
    [InlineData(Core.ContentType.Json, "Text")]
    [InlineData(Core.ContentType.Json, "\"Field\":\"Value\"")]
    [InlineData(Core.ContentType.Xml, "")]
    [InlineData(Core.ContentType.Xml, "Text")]
    [InlineData(Core.ContentType.Xml, "<data></value>")]
    public async Task FormatStructuredText_When_Invalid(ContentType contentType, string stringContent) {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            ContentType = contentType,
            StringContent = stringContent
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.FormatStructuredText();

        Assert.Equal(stringContent, subject.StringContent.Value);

        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task PickFileContent() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string fileContents = "FileContent";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(fileContents));
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "POST",
            Url = "https://localhost",
            ContentType = Core.ContentType.File,
            FileContent = "PreviousFileContent"
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.PickFileContent();

        Assert.Equal(fileContents, subject.FileContent.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == subject.FileContent));
    }

    [Fact]
    public async Task PickFileContent_Does_Nothing_Without_Selected_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns((Storage.FileResult?)null);
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "POST",
            Url = "https://localhost",
            ContentType = Core.ContentType.File,
            FileContent = "PreviousFileContent"
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, new(filePath), new(), request);

        await subject.PickFileContent();

        Assert.Equal("PreviousFileContent", subject.FileContent.Value);
        messageService.DidNotReceive().Send(Arg.Any<ValidatablePropertyUpdatedMessage>());
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
    public async Task ToggleHasManualContentTypeHeader() {
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
            HasManualContentTypeHeader = { Value = true }
        };

        subject.ToggleHasManualContentTypeHeader();

        Assert.False(subject.HasManualContentTypeHeader.Value);
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
