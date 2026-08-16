using NSubstitute;
using System;
using System.IO.Abstractions;

namespace RequestFiend.Core.Tests;

public class TestsBase {
    protected static readonly IFileSystem fileSystem;
    protected static readonly IPasswordProvider passwordProvider;
    protected static readonly ISecretEncryptor secretEncryptor;

    static TestsBase() {
        fileSystem = Substitute.For<IFileSystem>();
        passwordProvider = Substitute.For<IPasswordProvider>();
        secretEncryptor = Substitute.For<ISecretEncryptor>();
        
        AppHost.Services = Substitute.For<IServiceProvider>();        
        AppHost.Services.GetService(typeof(IFileSystem)).Returns(fileSystem);
        AppHost.Services.GetService(typeof(IPasswordProvider)).Returns(passwordProvider);
        AppHost.Services.GetService(typeof(ISecretEncryptor)).Returns(secretEncryptor);
    }
}
