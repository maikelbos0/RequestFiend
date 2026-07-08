using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class EnvironmentServiceTests {
    [Fact]
    public async Task Save() {
        const string filePath = @"C:\Documents\Local.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "DefaultHeader", Value = "Accept" }
            }
        };

        var subject = new EnvironmentService(fileSystem, Substitute.For<IPreferencesService>(), Substitute.For<IPopupService>());

        await subject.Save(filePath, environment);

        await fileSystem.Received(1).File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(environment), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetActiveEnvironment() {
        const string filePath = @"C:\Documents\Local.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "DefaultHeader", Value = "Accept" }
            }
        };
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(environment));
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetActiveEnvironment().Returns(new FileModel(filePath));
        var popupService = Substitute.For<IPopupService>();

        var subject = new EnvironmentService(fileSystem, preferencesService, popupService);

        var result = await subject.GetActiveEnvironment();

        Assert.Single(result.Variables);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task GetActiveEnvironment_Caches() {
        const string filePath = @"C:\Documents\Local.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "DefaultHeader", Value = "Accept" }
            }
        };
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(environment));
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetActiveEnvironment().Returns(new FileModel(filePath));
        var popupService = Substitute.For<IPopupService>();

        var subject = new EnvironmentService(fileSystem, preferencesService, popupService);

        _ = await subject.GetActiveEnvironment();
        _ = await subject.GetActiveEnvironment();

        preferencesService.Received(1).GetActiveEnvironment();
        await fileSystem.File.Received(1).ReadAllTextAsync(filePath, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetActiveEnvironment_Resets_After_Receiving_ActiveEnvironmentChangedMessage() {
        const string filePath = @"C:\Documents\Local.json";

        var fileSystem = Substitute.For<IFileSystem>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "DefaultHeader", Value = "Accept" }
            }
        };
        fileSystem.File.Exists(filePath).Returns(true);
        fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns(JsonSerializer.Serialize(environment));
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetActiveEnvironment().Returns(new FileModel(filePath));
        var popupService = Substitute.For<IPopupService>();

        var subject = new EnvironmentService(fileSystem, preferencesService, popupService);

        _ = await subject.GetActiveEnvironment();
        subject.Receive(new ActiveEnvironmentChangedMessage());
        _ = await subject.GetActiveEnvironment();

        preferencesService.Received(2).GetActiveEnvironment();
        await fileSystem.File.Received(2).ReadAllTextAsync(filePath, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetActiveEnvironment_Without_Active_Environment() {
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetActiveEnvironment().ReturnsNull();
        var popupService = Substitute.For<IPopupService>();

        var subject = new EnvironmentService(Substitute.For<IFileSystem>(), preferencesService, popupService);

        var result = await subject.GetActiveEnvironment();

        Assert.Empty(result.Variables);
        await popupService.DidNotReceive().ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task GetActiveEnvironment_Returns_Empty_For_Missing_File() {
        const string filePath = @"C:\Documents\Local.json";

        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(false);
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetActiveEnvironment().Returns(new FileModel(filePath));
        var popupService = Substitute.For<IPopupService>();

        var subject = new EnvironmentService(fileSystem, preferencesService, popupService);

        var result = await subject.GetActiveEnvironment();

        Assert.Empty(result.Variables);
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }

    [Fact]
    public async Task GetActiveEnvironment_Returns_Empty_For_Deserialization_Error() {
        const string filePath = @"C:\Documents\Local.json";

        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists(filePath).Returns(true); fileSystem.File.ReadAllTextAsync(filePath, Arg.Any<CancellationToken>()).Returns("Invalid JSON");
        var preferencesService = Substitute.For<IPreferencesService>();
        preferencesService.GetActiveEnvironment().Returns(new FileModel(filePath));
        var popupService = Substitute.For<IPopupService>();

        var subject = new EnvironmentService(fileSystem, preferencesService, popupService);

        var result = await subject.GetActiveEnvironment();

        Assert.Empty(result.Variables);
        await popupService.Received(1).ShowErrorPopup(Arg.Any<string>());
    }
}
