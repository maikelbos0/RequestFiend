using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Models.Tests;

public class RequestLogModelTests {
    [Fact]
    public void Constructor() {
        var subject = new RequestLogModel(1);

        Assert.Equal("Request log", subject.PageTitleBase);
        Assert.Equal("Request log", subject.ShellItemTitleBase);

        Assert.Equal([], subject.Validatables);
    }

    [Fact]
    public async Task Add_And_StartUpdating() {
        var subject = new RequestLogModel(1);
        var cancellationTokenSource = new CancellationTokenSource();
        var updatingTask = subject.StartUpdating(cancellationTokenSource.Token);

        subject.Add($"Test 1{Environment.NewLine}");
        subject.Add($"Test 2{Environment.NewLine}");

        await Task.Delay(10, TestContext.Current.CancellationToken);

        cancellationTokenSource.Cancel();
        await updatingTask;

        Assert.Equal($"Test 1{Environment.NewLine}Test 2{Environment.NewLine}", subject.LogEvents);
    }

    [Fact]
    public async Task Clear_And_StartUpdating() {
        var subject = new RequestLogModel(1);
        var cancellationTokenSource = new CancellationTokenSource();
        var updatingTask = subject.StartUpdating(cancellationTokenSource.Token);

        subject.Add($"Test 1{Environment.NewLine}");
        subject.Add($"Test 2{Environment.NewLine}");

        subject.Clear();

        cancellationTokenSource.Cancel();
        await updatingTask;
        
        await Task.Delay(10, TestContext.Current.CancellationToken);

        Assert.Empty(subject.LogEvents);
    }
}
