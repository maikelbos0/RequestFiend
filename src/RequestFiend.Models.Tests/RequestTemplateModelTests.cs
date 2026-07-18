using CommunityToolkit.Maui.Core;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Storage = Microsoft.Maui.Storage;

namespace RequestFiend.Models.Tests;

public class RequestTemplateModelTests {
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

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            Name = { Value = "NewName" }
        };

        subject.Name.Set();

        Assert.Equal("NewName", request.Name);
    }

    [Fact]
    public void Method() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            Method = { Value = "POST" }
        };

        subject.Method.Set();

        Assert.Equal("POST", request.Method);
    }

    [Fact]
    public void Url() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            Url = { Value = "https://url" }
        };

        subject.Url.Set();

        Assert.Equal("https://url", request.Url);
    }

    [Theory]
    [InlineData(Core.ContentType.None, false, false, false, false, false, false)]
    [InlineData(Core.ContentType.Text, true, false, true, true, false, false)]
    [InlineData(Core.ContentType.Json, true, true, false, true, false, false)]
    [InlineData(Core.ContentType.Xml, true, true, false, true, false, false)]
    [InlineData(Core.ContentType.File, true, false, true, false, true, false)]
    [InlineData(Core.ContentType.FormData, true, false, true, false, false, true)]
    public void ContentType(
        ContentType contentType,
        bool expectedUsesContent,
        bool expectedUsesStructuredContent,
        bool expectedUsesUnstructuredContent,
        bool expectedUsesStringContent,
        bool expectedUsesFileContent,
        bool expectedUsesFormDataContent
    ) {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            ContentType = { Value = Options.ContentTypeMap[contentType] }
        };
        Assert.Equal(expectedUsesContent, subject.UsesContent);
        Assert.Equal(expectedUsesStructuredContent, subject.UsesStructuredContent);
        Assert.Equal(expectedUsesUnstructuredContent, subject.UsesUnstructuredContent);
        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesFileContent, subject.UsesFileContent);
        Assert.Equal(expectedUsesFormDataContent, subject.UsesFormDataContent);

        subject.ContentType.Set();

        Assert.Equal(contentType, request.ContentType);
    }

    [Fact]
    public void HasManualContentTypeHeader() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            HasManualContentTypeHeader = { Value = true }
        };

        subject.HasManualContentTypeHeader.Set();

        Assert.True(request.HasManualContentTypeHeader);
    }

    [Fact]
    public void StringContent() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            StringContent = { Value = "StringContent" }
        };

        subject.StringContent.Set();

        Assert.Equal("StringContent", request.StringContent);
    }

    [Fact]
    public void FileContent() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            FileContent = { Value = "FileContent" }
        };

        subject.FileContent.Set();

        Assert.Equal("FileContent", request.FileContent);
    }

    [Theory]
    [InlineData(Core.ContentType.None, false, false, false, false, false, false)]
    [InlineData(Core.ContentType.Text, true, false, true, true, false, false)]
    [InlineData(Core.ContentType.Json, true, true, false, true, false, false)]
    [InlineData(Core.ContentType.Xml, true, true, false, true, false, false)]
    [InlineData(Core.ContentType.File, true, false, true, false, true, false)]
    [InlineData(Core.ContentType.FormData, true, false, true, false, false, true)]
    public void Constructor(
        ContentType contentType,
        bool expectedUsesContent,
        bool expectedUsesStructuredContent,
        bool expectedUsesUnstructuredContent,
        bool expectedUsesStringContent,
        bool expectedUsesFileContent,
        bool expectedUsesFormDataContent
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
                new() { Name = "Name", Value = "Value" },
                new() { Name = "Key", Value = "Value" }
            },
            FormFileContent = {
                new() { Name = "Data", Value = "Value" },
                new() { Name = "File", Value = "Value" },
                new() { Name = "Image", Value = "Value" }
            },
            PreExchangeScript = { Code = "PreExchangeScript" },
            PostExchangeScript = { Code = "PostExchangeScript" },
            OnExceptionScript = { Code = "OnExceptionScript" }
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request);

        Assert.Equal($"{Path.GetFileNameWithoutExtension(filePath)} - {request.Name}", subject.PageTitleBase);
        Assert.Equal(request.Name, subject.ShellItemTitleBase);

        Assert.Equal(new FileModel(filePath), subject.File);
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
        Assert.Equal(request.FormFileContent.Count, subject.FormFileContent.Count);
        Assert.Equal(request.PreExchangeScript.Code, subject.PreExchangeScript.Code.Value);
        Assert.Equal(request.PostExchangeScript.Code, subject.PostExchangeScript.Code.Value);
        Assert.Equal(request.OnExceptionScript.Code, subject.OnExceptionScript.Code.Value);

        Assert.Equal(expectedUsesContent, subject.UsesContent);
        Assert.Equal(expectedUsesStructuredContent, subject.UsesStructuredContent);
        Assert.Equal(expectedUsesUnstructuredContent, subject.UsesUnstructuredContent);
        Assert.Equal(expectedUsesStringContent, subject.UsesStringContent);
        Assert.Equal(expectedUsesFileContent, subject.UsesFileContent);
        Assert.Equal(expectedUsesFormDataContent, subject.UsesFormDataContent);

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
            subject.FormFileContent,
            subject.PreExchangeScript,
            subject.PostExchangeScript,
            subject.OnExceptionScript
        ], subject.Validatables);
    }

    [Fact]
    public async Task CreateExchange() {
        const string filePath = @"C:\Documents\External data requests.json";

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
        var environmentService = Substitute.For<IEnvironmentService>();
        environmentService.GetActiveEnvironment().Returns(new Environment());

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, environmentService, new(filePath), collection, request) {
            Name = { Value = "ChangedName" }
        };

        await subject.CreateExchange();

        messageService.Received(1).Send(Arg.Is<CreateExchangeMessage>(message => message.FilePath == filePath && message.Id == subject.Id && message.Collection == collection && message.Request.Name == subject.Name.Value));
    }

    [Theory]
    [InlineData("", "", "", "", "", "", "", "", "")]
    [InlineData("", "POST", "https://localhost", "Name", "JSON", "", "Name", "", "")]
    [InlineData("Name", "", "https://localhost", "Name", "JSON", "", "Name", "", "")]
    [InlineData("Name", "POST", "", "Name", "JSON", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "", "JSON", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "Name", "", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "Name", "File", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "Name", "Multipart form data", "", "", "Name", "FileContent")]
    [InlineData("Name", "POST", "https://localhost", "Name", "Multipart form data", "", "Name", "", "FileContent")]
    [InlineData("Name", "POST", "https://localhost", "Name", "Multipart form data", "", "Name", "Name", "")]
    public async Task CreateExchange_Fails_When_Invalid(string name, string method, string url, string headerName, string contentType, string fileContent, string formFieldName, string formFileName, string formFileValue) {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            FormFieldContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            FormFileContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.ContentType.Value = contentType;
        subject.FileContent.Value = fileContent;
        subject.FormFieldContent[0].Name.Value = formFieldName;
        subject.FormFileContent[0].Name.Value = formFileName;
        subject.FormFileContent[0].Value.Value = formFileValue;

        await subject.CreateExchange();

        messageService.DidNotReceive().Send(Arg.Any<CreateExchangeMessage>());
    }

    [Fact]
    public void CreateRequest() {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "",
            Method = "",
            Url = ""
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            Name = { Value = "Name" },
            Method = { Value = "GET" },
            Url = { Value = "https://localhost" },
            Headers = {
                new() { Name = "Name", Value = "Value" }
            },
            ContentType = { Value = Options.ContentTypeMap[Core.ContentType.Json] },
            HasManualContentTypeHeader = { Value = true },
            StringContent = { Value = "StringContent" },
            FileContent = { Value = "FileContent" },
            FormFieldContent = {
                new() { Name = "Name", Value = "Value" },
                new() { Name = "Key", Value = "Value" }
            },
            FormFileContent = {
                new() { Name = "Data", Value = "Value" },
                new() { Name = "File", Value = "Value" },
                new() { Name = "Image", Value = "Value" }
            },
            PreExchangeScript = { Code = { Value = "PreExchangeScript" } },
            PostExchangeScript = { Code = { Value = "PostExchangeScript" } },
            OnExceptionScript = { Code = { Value = "OnExceptionScript" } }
        };

        var result = subject.CreateRequest();

        Assert.Equal(result.Name, subject.Name.Value);
        Assert.Equal(result.Method, subject.Method.Value);
        Assert.Equal(result.Url, subject.Url.Value);
        Assert.Equal(result.Headers.Count, subject.Headers.Count);
        Assert.Equal(Options.ContentTypeMap[result.ContentType], subject.ContentType.Value);
        Assert.Equal(result.HasManualContentTypeHeader, subject.HasManualContentTypeHeader.Value);
        Assert.Equal(result.StringContent, subject.StringContent.Value);
        Assert.Equal(result.FileContent, subject.FileContent.Value);
        Assert.Equal(result.FormFieldContent.Count, subject.FormFieldContent.Count);
        Assert.Equal(result.FormFileContent.Count, subject.FormFileContent.Count);
        Assert.Equal(result.PreExchangeScript.Code, subject.PreExchangeScript.Code.Value);
        Assert.Equal(result.PostExchangeScript.Code, subject.PostExchangeScript.Code.Value);
        Assert.Equal(result.OnExceptionScript.Code, subject.OnExceptionScript.Code.Value);
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\External data requests.json";

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

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, Substitute.For<IEnvironmentService>(), new(filePath), collection, request);

        subject.Name.Value = "Name";

        await subject.Update();

        Assert.False(subject.IsModified);

        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData("", "", "", "", "", "", "", "", "")]
    [InlineData("", "POST", "https://localhost", "Name", "JSON", "", "Name", "", "")]
    [InlineData("Name", "", "https://localhost", "Name", "JSON", "", "Name", "", "")]
    [InlineData("Name", "POST", "", "Name", "JSON", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "", "JSON", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "Name", "", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "Name", "File", "", "Name", "", "")]
    [InlineData("Name", "POST", "https://localhost", "Name", "Multipart form data", "", "", "Name", "FileContent")]
    [InlineData("Name", "POST", "https://localhost", "Name", "Multipart form data", "", "Name", "", "FileContent")]
    [InlineData("Name", "POST", "https://localhost", "Name", "Multipart form data", "", "Name", "Name", "")]
    public async Task Update_Fails_When_Invalid(string name, string method, string url, string headerName, string contentType, string fileContent, string formFieldName, string formFileName, string formFileValue) {
        const string filePath = @"C:\Documents\External data requests.json";

        var requestTemplateCollectionService = Substitute.For<IRequestTemplateCollectionService>();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Old",
            Method = "POST",
            Url = "https://previous",
            Headers = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            FormFieldContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            FormFileContent = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, Substitute.For<IPopupService>(), messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

        subject.Name.Value = name;
        subject.Method.Value = method;
        subject.Url.Value = url;
        subject.Headers[0].Name.Value = headerName;
        subject.ContentType.Value = contentType;
        subject.FileContent.Value = fileContent;
        subject.FormFieldContent[0].Name.Value = formFieldName;
        subject.FormFileContent[0].Name.Value = formFileName;
        subject.FormFileContent[0].Value.Value = formFileValue;

        await subject.Update();

        Assert.True(subject.IsModified);

        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task ShowUrlPopup() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string url = "https://localhost";
        const string expectedUrl = "https://localhost/api";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = url
        };
        popupResult.Result.Returns(expectedUrl);
        popupService.ShowUrlPopup(collection, url).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), collection, request);

        await subject.ShowUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, url);
        Assert.Equal(expectedUrl, subject.Url.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == subject.Url));
    }

    [Fact]
    public async Task ShowUrlPopup_Does_Nothing_Without_Result() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string url = "https://localhost";

        var popupService = Substitute.For<IPopupService>();
        var popupResult = Substitute.For<IPopupResult<string>>();
        var collection = new RequestTemplateCollection();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = url
        };
        popupResult.Result.ReturnsNull();
        popupService.ShowUrlPopup(collection, url).Returns(popupResult);
        var messageService = Substitute.For<IMessageService>();

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), collection, request);

        await subject.ShowUrlPopup();

        await popupService.Received(1).ShowUrlPopup(collection, url);
        Assert.Equal(url, subject.Url.Value);
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

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

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

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

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

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

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

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

        await subject.FormatStructuredText();

        Assert.Equal(stringContent, subject.StringContent.Value);

        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task PickFormFileContent() {
        const string filePath = @"C:\Documents\External data requests.json";
        const string fileContents = "FileContent";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).Returns(new Storage.FileResult(fileContents));
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "POST",
            Url = "https://localhost"
        };
        var pair = new NameValuePairModel(new() { Name = "Name", Value = "PreviousValue" }, Validator.Required);

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

        await subject.PickFormFileContent(pair);

        Assert.Equal(fileContents, pair.Value.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == pair.Value));
    }

    [Fact]
    public async Task PickFormFileContent_Does_Nothing_Without_Selected_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).ReturnsNull();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "POST",
            Url = "https://localhost"
        };

        var pair = new NameValuePairModel(new() { Name = "Name", Value = "PreviousValue" }, Validator.Required);

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

        await subject.PickFormFileContent(pair);

        Assert.Equal("PreviousValue", pair.Value.Value);
        messageService.DidNotReceive().Send(Arg.Any<ValidatablePropertyUpdatedMessage>());
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

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

        await subject.PickFileContent();

        Assert.Equal(fileContents, subject.FileContent.Value);
        messageService.Received(1).Send(Arg.Is<ValidatablePropertyUpdatedMessage>(message => message.Property == subject.FileContent));
    }

    [Fact]
    public async Task PickFileContent_Does_Nothing_Without_Selected_File() {
        const string filePath = @"C:\Documents\External data requests.json";

        var popupService = Substitute.For<IPopupService>();
        popupService.ShowPickFileDialog(Arg.Any<Storage.PickOptions>()).ReturnsNull();
        var messageService = Substitute.For<IMessageService>();
        var request = new RequestTemplate() {
            Name = "Name",
            Method = "POST",
            Url = "https://localhost",
            ContentType = Core.ContentType.File,
            FileContent = "PreviousFileContent"
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), new(), request);

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
            Requests = { request }
        };

        var subject = new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), collection, request);

        await subject.Delete();

        Assert.Empty(collection.Requests);
        await requestTemplateCollectionService.Received(1).Save(filePath, collection);
        messageService.Received(1).Send(Arg.Any<RequestTemplateDeletedMessage>(), subject.Id);
        messageService.Received(1).Send(Arg.Is<RequestTemplateRemovedFromCollectionMessage>(message => message.Request == request), new FileModel(filePath));
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task ToggleHasManualContentTypeHeader(bool initialValue, bool expectedValue) {
        const string filePath = @"C:\Documents\External data requests.json";

        var request = new RequestTemplate() {
            Name = "Name",
            Method = "GET",
            Url = "https://localhost"
        };
        var collection = new RequestTemplateCollection() {
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), Substitute.For<IPopupService>(), Substitute.For<IMessageService>(), Substitute.For<IEnvironmentService>(), new(filePath), collection, request) {
            HasManualContentTypeHeader = { Value = initialValue }
        };

        subject.ToggleHasManualContentTypeHeader();

        Assert.Equal(expectedValue, subject.HasManualContentTypeHeader.Value);
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
            Requests = { request }
        };

        var subject = new RequestTemplateModel(Substitute.For<IRequestTemplateCollectionService>(), popupService, messageService, Substitute.For<IEnvironmentService>(), new(filePath), collection, request);

        await subject.Delete();

        Assert.Equal(request, Assert.Single(collection.Requests));
        await requestTemplateCollectionService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<RequestTemplateCollection>());
        messageService.DidNotReceive().Send(Arg.Any<RequestTemplateDeletedMessage>(), Arg.Any<System.Guid>());
        messageService.DidNotReceive().Send(Arg.Any<RequestTemplateRemovedFromCollectionMessage>(), Arg.Any<FileModel>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
