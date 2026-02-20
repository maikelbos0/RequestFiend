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
    }

    //[Theory]
    //[InlineData(false, false, false, false, "External data requests", "External data requests")]
    //[InlineData(true, false, true, false, "External data requests", "External data requests")]
    //[InlineData(true, true, true, false, "External data requests", "External data requests")]
    //[InlineData(false, true, false, true, "External data requests ●", "External data requests ●")]
    //public void Changing_Child_State_Updates_State(bool childHasError, bool childIsModified, bool expectedHasError, bool expectedIsModified, string expectedPageTitle, string expectedShellItemTitle) {
    //    const string filePath = @"C:\Documents\External data requests.json";

    //    var subject = new RequestTemplateCollectionModel(new(filePath));
    //    var child = new BoundModelBase("Child", "Child");

    //    subject.AddChild(child);

    //    child.HasError = childHasError;
    //    child.IsModified = childIsModified;

    //    Assert.Equal(expectedHasError, subject.HasError);
    //    Assert.Equal(expectedIsModified, subject.IsModified);
    //    Assert.Equal(expectedPageTitle, subject.PageTitle);
    //    Assert.Equal(expectedShellItemTitle, subject.ShellItemTitle);
    //}
}
