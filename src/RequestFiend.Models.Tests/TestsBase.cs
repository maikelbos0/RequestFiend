using NSubstitute;
using RequestFiend.Core;
using System;

namespace RequestFiend.Models.Tests;

public class TestsBase {
    static TestsBase() {
        AppHost.Services = Substitute.For<IServiceProvider>();
    }
}
