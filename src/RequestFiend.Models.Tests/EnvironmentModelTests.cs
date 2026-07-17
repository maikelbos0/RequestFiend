using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class EnvironmentModelTests {
    [Fact]
    public void Constructor() {
        const string filePath = @"C:\Documents\Local.json";

        var environment = new Environment() {
            Variables = {
                new() { Name = "Foo", Value = "Bar" }
            }
        };
        
        var subject = new EnvironmentModel(Substitute.For<System.Func<CancellationToken, Task>>(), Substitute.For<IEnvironmentService>(), new(filePath), environment);

        Assert.Equal(environment.Variables.Count, subject.Variables.Count);

        Assert.Equal([subject.Variables], subject.Validatables);
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\Local.json";

        var closeMethod = Substitute.For<System.Func<CancellationToken, Task>>();
        var environmentService = Substitute.For<IEnvironmentService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };

        var subject = new EnvironmentModel(closeMethod, environmentService, new(filePath), environment);

        subject.Variables[0].Name.Value = "Name";
        subject.Variables[0].Value.Value = "Value";

        await subject.Update(CancellationToken.None);

        Assert.False(subject.IsModified);

        await environmentService.Received(1).Save(filePath, environment);
        await closeMethod.Received().Invoke(CancellationToken.None);
    }

    [Fact]
    public async Task Update_Fails_When_Invalid() {
        const string filePath = @"C:\Documents\Local.json";
        
        var closeMethod = Substitute.For<System.Func<CancellationToken, Task>>();
        var environmentService = Substitute.For<IEnvironmentService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "PreviousName" }
            }
        };

        var subject = new EnvironmentModel(closeMethod, environmentService, new(filePath), environment);

        subject.Variables[0].Name.Value = "";

        await subject.Update(CancellationToken.None);

        Assert.True(subject.IsModified);

        await environmentService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<Environment>());
        await closeMethod.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }
}
