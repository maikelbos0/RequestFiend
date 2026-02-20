using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.IO;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestTemplateCollectionModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection() {
            Requests = [
                new() { Name = "Request", Method = "GET", Url = "https://localhost" },
            ]
        };

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            new(filePath),
            collection
        );

        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), subject.PageTitleBase);
        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), subject.ShellItemTitleBase);
        Assert.NotNull(subject.Settings);
        Assert.NotNull(subject.NewRequest);
        Assert.NotNull(Assert.Single(subject.Requests));
    }

    [Fact]
    public void AddRequest() {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            new(filePath),
            collection
        );

        var request = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost"
        };

        var result = subject.AddRequest(request);

        Assert.NotNull(Assert.Single(subject.Requests));
    }

    [Theory]
    [InlineData(false, false, false, false, false, "External data requests", "External data requests")]
    [InlineData(true, false, true, false, false, "External data requests", "External data requests")]
    [InlineData(true, true, true, true, false, "External data requests ●", "External data requests ●")]
    [InlineData(false, true, false, true, true, "External data requests ●", "External data requests ●")]
    public void Changing_Settings_State_Updates_State(bool settingsHasError, bool settingsIsModified, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError, string expectedPageTitle, string expectedShellItemTitle) {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            new(filePath),
            collection
        );

        subject.Settings.HasError = settingsHasError;
        subject.Settings.IsModified = settingsIsModified;

        // Required since it defaults to true
        subject.NewRequest.HasError = false;

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Theory]
    [InlineData(false, false, false, false, false, "External data requests", "External data requests")]
    [InlineData(true, false, true, false, false, "External data requests", "External data requests")]
    [InlineData(true, true, true, true, false, "External data requests ●", "External data requests ●")]
    [InlineData(false, true, false, true, true, "External data requests ●", "External data requests ●")]
    public void Changing_NewRequeset_State_Updates_State(bool newRequestHasError, bool newRequestIsModified, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError, string expectedPageTitle, string expectedShellItemTitle) {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            new(filePath),
            collection
        );

        subject.NewRequest.HasError = newRequestHasError;
        subject.NewRequest.IsModified = newRequestIsModified;

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }

    [Theory]
    [InlineData(false, false, false, false, false, "External data requests", "External data requests")]
    [InlineData(true, false, true, false, false, "External data requests", "External data requests")]
    [InlineData(true, true, true, true, false, "External data requests ●", "External data requests ●")]
    [InlineData(false, true, false, true, true, "External data requests ●", "External data requests ●")]
    public void Changing_Request_State_Updates_State(bool requestHasError, bool requestIsModified, bool expectedHasError, bool expectedIsModified, bool expectedIsModifiedWithoutError, string expectedPageTitle, string expectedShellItemTitle) {
        const string filePath = @"C:\Documents\External data requests.json";

        var collection = new RequestTemplateCollection();

        var subject = new RequestTemplateCollectionModel(
            Substitute.For<IRequestTemplateCollectionService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<IMessageService>(),
            new(filePath),
            collection
        );

        var request = subject.AddRequest(new() { Name = "Request", Method = "GET", Url = "https://localhost" });

        request.HasError = requestHasError;
        request.IsModified = requestIsModified;

        // Required since it defaults to true
        subject.NewRequest.HasError = false;

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedIsModifiedWithoutError, subject.IsModifiedWithoutError);
        Assert.Equal(expectedPageTitle, subject.PageTitle);
        Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    }
}
