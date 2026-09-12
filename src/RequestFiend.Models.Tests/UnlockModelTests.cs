using NSubstitute;
using RequestFiend.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class UnlockModelTests : TestsBase {
    [Fact]
    public void Constructor() {
        var subject = new UnlockModel(Substitute.For<Func<CancellationToken, Task>>(), Substitute.For<ISecretEncryptor>(), Substitute.For<ISecretOwner>());
        
        Assert.Empty(subject.Password.Value);

        Assert.Equal([subject.Password], subject.Validatables);
    }

    [Fact]
    public async Task TryUnlock_With_Valid_Password() {
        var closeMethod = Substitute.For<Func<CancellationToken, Task>>();
        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryUnlock(Arg.Any<ISecretOwner>(), Arg.Any<string>()).Returns(true);

        var subject = new UnlockModel(closeMethod, secretEncryptor, owner) {
            Password = { Value = "password" }
        };

        await subject.TryUnlock(CancellationToken.None);

        secretEncryptor.Received().TryUnlock(owner, "password");

        await closeMethod.Received().Invoke(CancellationToken.None);
    }

    [Fact]
    public async Task TryUnlock_With_Invalid_Password() {
        var closeMethod = Substitute.For<Func<CancellationToken, Task>>();
        var secretEncryptor = Substitute.For<ISecretEncryptor>();
        var owner = Substitute.For<ISecretOwner>();
        secretEncryptor.TryUnlock(Arg.Any<ISecretOwner>(), Arg.Any<string>()).Returns(false);

        var subject = new UnlockModel(closeMethod, secretEncryptor, owner) {
            Password = { Value = "password" }
        };

        await subject.TryUnlock(CancellationToken.None);

        secretEncryptor.Received().TryUnlock(owner, "password");

        await closeMethod.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }
}
