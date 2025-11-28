using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelCollectionTests {
    [Fact]
    public void Constructor() {
        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };

        var subject = new NameValuePairModelCollection(collection);

        Assert.Equal(collection.Count, subject.Count);

        for(var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
        }
    }

    [Fact]
    public void Reinitialize() {
        var subject = new NameValuePairModelCollection([]) {
            new(),
            new()
        };

        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };

        subject.Reinitialize(collection);

        Assert.Equal(collection.Count, subject.Count);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void Reinitialize_Throws_For_Wrong_Length(int collectionLength) {
        var subject = new NameValuePairModelCollection([]) {
            new(),
            new()
        };

        var collection = Enumerable.Range(0, collectionLength)
            .Select(_ => new NameValuePair() { Name = "Name", Value = "Value" })
            .ToList();

        Assert.Throws<ArgumentException>(() => subject.Reinitialize(collection));
    }
}
