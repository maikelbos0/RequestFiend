using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace RequestFiend.Models.Tests.Services;

public class ModelDataProviderTests {
    private record DataType1();
    private record DataType2();
    private record DataType3();

    [Fact]
    public void CreateScope_And_GetData() {
        var value1 = new DataType1();
        var value2 = new DataType2();
        var value3 = new DataType3();

        var subject = new ModelDataProvider();

        using var scope1 = subject.CreateScope(value1);
        using var scope2 = subject.CreateScope(value2, value3);

        Assert.Equal(value1, subject.GetData<DataType1>());
        Assert.Equal(value2, subject.GetData<DataType2>());
        Assert.Equal(value3, subject.GetData<DataType3>());
    }

    [Fact]
    public void CreateScope_Throws_For_Same_Type() {
        var subject = new ModelDataProvider();

        using var scope1 = subject.CreateScope(new DataType1());
            
        Assert.Throws<ArgumentException>(() => subject.CreateScope(new DataType1()));
    }

    [Fact]
    public void GetData_Throws_For_Missing_Type() {
        var subject = new ModelDataProvider();

        Assert.Throws<KeyNotFoundException>(subject.GetData<DataType1>);
    }
}
