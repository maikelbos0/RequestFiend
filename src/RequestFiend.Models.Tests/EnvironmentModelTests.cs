using CommunityToolkit.Maui.Core;
using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class EnvironmentModelTests : TestsBase {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\Local.json";

        var environment = new Environment() {
            Variables = {
                new() { Name = "Foo", Value = "Bar" }
            },
            Secrets = {
                new() { Name = "PreviousName" }
            }
        };

        var subject = new EnvironmentModel(
            Substitute.For<System.Func<CancellationToken, Task>>(),
            Substitute.For<IEnvironmentService>(),
            Substitute.For<IPopupService>(),
            Substitute.For<ISecretEncryptor>(),
            new(filePath),
            environment
        );

        Assert.Equal(environment.Variables.Count, subject.Variables.Count);
        Assert.Equal(environment.Secrets.Count, subject.Secrets.Count);

        Assert.Equal([subject.Variables, subject.Secrets], subject.Validatables);
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\Local.json";

        var closeMethod = Substitute.For<System.Func<CancellationToken, Task>>();
        var environmentService = Substitute.For<IEnvironmentService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            },
            Secrets = {
                new() { Name = "PreviousName" }
            }
        };

        var subject = new EnvironmentModel(
            closeMethod,
            environmentService,
            Substitute.For<IPopupService>(),
            Substitute.For<ISecretEncryptor>(),
            new(filePath),
            environment
        );

        subject.Variables[0].Name.Value = "Name";
        subject.Variables[0].Value.Value = "Value";

        await subject.Update(CancellationToken.None);

        Assert.False(subject.IsModified);

        await environmentService.Received(1).Save(filePath, environment);
        await closeMethod.Received().Invoke(CancellationToken.None);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("", "Name")]
    [InlineData("Name", "")]
    public async Task Update_Fails_When_Invalid(string variableName, string secretName) {
        const string filePath = @"C:\Documents\Local.json";

        var closeMethod = Substitute.For<System.Func<CancellationToken, Task>>();
        var environmentService = Substitute.For<IEnvironmentService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "PreviousName" }
            },
            Secrets = {
                new() { Name = "PreviousName" }
            }
        };

        var subject = new EnvironmentModel(
            closeMethod,
            environmentService,
            Substitute.For<IPopupService>(),
            Substitute.For<ISecretEncryptor>(),
            new(filePath),
            environment
        );

        subject.Variables[0].Name.Value = variableName;
        subject.Secrets[0].Name.Value = secretName;

        await subject.Update(CancellationToken.None);

        Assert.True(subject.IsModified);

        await environmentService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<Environment>());
        await closeMethod.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unlock() {
        const string filePath = @"C:\Documents\Local.json";

        var environment = new Environment();
        var popupResult = Substitute.For<IPopupResult<bool>>();
        popupResult.Result.Returns(true);
        var popupService = Substitute.For<IPopupService>();
        popupService.ShowUnlockPopup(Arg.Any<ISecretEncryptor>(), Arg.Any<ISecretOwner>()).Returns(popupResult);
        var secretEncryptor = Substitute.For<ISecretEncryptor>();

        var subject = new EnvironmentModel(
            Substitute.For<System.Func<CancellationToken, Task>>(),
            Substitute.For<IEnvironmentService>(),
            popupService,
            secretEncryptor,
            new(filePath),
            environment
        ) {
            IsLocked = true
        };

        await subject.Unlock();

        Assert.False(subject.IsLocked);

        await popupService.Received().ShowUnlockPopup(secretEncryptor, environment);
    }

    [Fact]
    public void Lock() {
        const string filePath = @"C:\Documents\Local.json";

        var environment = new Environment();
        var secretEncryptor = Substitute.For<ISecretEncryptor>();

        var subject = new EnvironmentModel(
            Substitute.For<System.Func<CancellationToken, Task>>(),
            Substitute.For<IEnvironmentService>(),
            Substitute.For<IPopupService>(),
            secretEncryptor,
            new(filePath),
            environment
        ) {
            IsLocked = false
        };

        subject.Lock();

        Assert.True(subject.IsLocked);

        secretEncryptor.Received().Lock(environment);
    }
}
