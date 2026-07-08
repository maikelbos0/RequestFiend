using NSubstitute;
using RequestFiend.Core;
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
        var preferencesService = Substitute.For<IPreferencesService>();
        var environment = new Environment() {
            Variables = {
                new() { Name = "DefaultHeader", Value = "Accept" }
            }
        };

        var subject = new EnvironmentService(fileSystem, preferencesService);

        await subject.Save(filePath, environment);

        await fileSystem.Received(1).File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(environment), Arg.Any<CancellationToken>());
    }
}
