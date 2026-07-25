using NSubstitute;
using RequestFiend.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Console.Tests;

public class CommandHandlerTests {
    [Fact]
    public async Task ExecuteRequests() {
        var exchangeHandler = Substitute.For<IExchangeHandler>();
        var collection = new RequestTemplateCollection() {
            Requests = {
                new() { Name = "Foo", Method = "GET", Url = "https://localhost" },
                new() { Name = "Bar", Method = "POST", Url = "https://localhost" }
            },
            Variables = {
                new() { Name = "First" }
            }
        };
        var options = new ExchangeOptions(true, null);
        var environment = new Environment() {
            Variables = {
                new() { Name = "Second" }
            }
        };

        var subject = new CommandHandler(exchangeHandler);

        await subject.ExecuteRequests(collection, options, environment, CancellationToken.None);

        Received.InOrder(() => {
            exchangeHandler.Execute(Arg.Is<RequestTemplateSnapshot>(request => request.Name == "Foo" && request.Variables.Variables.Count == 2), collection, options, CancellationToken.None);
            exchangeHandler.Execute(Arg.Is<RequestTemplateSnapshot>(request => request.Name == "Bar" && request.Variables.Variables.Count == 2), collection, options, CancellationToken.None);
        });
    }
}
