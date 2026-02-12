using RequestFiend.Models.Services;
using System;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class ModelDataProviderTests {
    [Fact]
    public void CreateScope_And_GetData() {
        const string value = "42";

        var subject = new ModelDataProvider<string>();

        using (subject.CreateScope(value)) {
            var result = subject.GetData();

            Assert.Equal(value, result);
        }
    }

    [Fact]
    public void CreateScope_Throws_For_Double_Scope() {
        var subject = new ModelDataProvider<string>();

        using (subject.CreateScope("42")) {
            Assert.Throws<InvalidOperationException>(() => subject.CreateScope("42"));
        }
    }

    [Fact]
    public void GetData_Throws_When_Outside_Scope() {
        var subject = new ModelDataProvider<string>();

        Assert.Throws<InvalidOperationException>(() => subject.GetData());
    }
}
