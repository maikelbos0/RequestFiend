using NSubstitute;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
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

        var subject = new EnvironmentModel(Substitute.For<IEnvironmentService>(), Substitute.For<IMessageService>(), new(filePath), environment);


        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), subject.PageTitleBase);
        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), subject.ShellItemTitle);

        Assert.Equal(new FileModel(filePath), subject.File);

        Assert.Equal(environment.Variables.Count, subject.Variables.Count);

        Assert.Equal([subject.Variables], subject.Validatables);
    }

    [Fact]
    public async Task Update() {
        const string filePath = @"C:\Documents\Local.json";

        var environmentService = Substitute.For<IEnvironmentService>();
        var messageService = Substitute.For<IMessageService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "PreviousName", Value = "PreviousValue" }
            }
        };

        var subject = new EnvironmentModel(environmentService, messageService, new(filePath), environment);

        subject.Variables[0].Name.Value = "Name";
        subject.Variables[0].Value.Value = "Value";

        await subject.Update();

        Assert.False(subject.Variables.IsModified);

        await environmentService.Received(1).Save(filePath, environment);
        messageService.Received(1).Send(Arg.Any<SuccessMessage>());
    }

    [Fact]
    public async Task Update_Fails_When_Invalid() {
        const string filePath = @"C:\Documents\Local.json";

        var environmentService = Substitute.For<IEnvironmentService>();
        var messageService = Substitute.For<IMessageService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "PreviousName" }
            }
        };

        var subject = new EnvironmentModel(environmentService, messageService, new(filePath), environment);

        subject.Variables[0].Name.Value = "";

        await subject.Update();

        await environmentService.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<Environment>());
        messageService.DidNotReceive().Send(Arg.Any<SuccessMessage>());
    }
}
