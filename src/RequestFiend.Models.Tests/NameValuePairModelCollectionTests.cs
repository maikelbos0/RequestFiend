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

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
        }
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("Value", false)]
    public void Constructor_HasError(string value, bool expectedHasError) {
        var collection = new List<NameValuePair>() {
            new() { Name = "Name", Value = value }
        };

        var subject = new NameValuePairModelCollection(collection);

        Assert.Equal(expectedHasError, subject.HasError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Value", false)]
    public void Add(string? value, bool expectedHasError) {
        var subject = new NameValuePairModelCollection([]) {
            new() {
                Name = { Value = "Name" },
                Value = { Value = value }
            }
        };

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.True(subject.HasItems);
    }

    [Fact]
    public void Remove() {
        var item = new NameValuePairModel() {
            Name = { Value = "Name" },
            Value = { Value = null }
        };

        var subject = new NameValuePairModelCollection([]) {
            item
        };

        subject.Remove(item);

        Assert.False(subject.HasError);
        Assert.False(subject.HasItems);
    }

    [Fact]
    public void OnRemoveClicked() {
        var pair = new NameValuePairModel();
        var subject = new NameValuePairModelCollection([]) {
            new(),
            pair,
            new()
        };

        subject.OnRemoveClicked(pair);

        Assert.Equal(2, subject.Count);
        Assert.DoesNotContain(pair, subject);
    }

    [Fact]
    public void OnAddClicked() {
        var subject = new NameValuePairModelCollection([]);

        subject.OnAddClicked();

        Assert.Single(subject);
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
