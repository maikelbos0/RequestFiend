using NSubstitute;
using System;
using System.IO.Abstractions;

namespace RequestFiend.Core.Tests;

public class TestsBase {
    static TestsBase() {
        AppHost.Services = Substitute.For<IServiceProvider>();
        AppHost.Services.GetService(typeof(IFileSystem)).Returns(Substitute.For<IFileSystem>());
    }
}
