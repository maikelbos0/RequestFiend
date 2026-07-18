using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelCollectionTests {
    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Constructor(bool isValid, bool expectedHasError) {
        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };
        var subject = new NameValuePairModelCollection(collection, _ => isValid, _ => isValid);

        Assert.Equal(collection.Count, subject.Count);
        Assert.False(subject.IsModified);
        Assert.True(subject.HasItems);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Equal(expectedHasError, subject[i].Name.HasError);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
            Assert.Equal(expectedHasError, subject[i].Value.HasError);
        }
    }

    [Theory]
    [InlineData("FirstName", 0, 0, false, false, true)]
    [InlineData("", 0, 0, true, true, true)]
    [InlineData("Name", 0, 0, false, true, true)]
    [InlineData("FirstName", 1, 0, true, true, true)]
    [InlineData("FirstName", 0, 1, false, true, true)]
    [InlineData("FirstName", 0, 2, false, true, false)]
    [InlineData("FirstName", 1, 2, true, true, true)]
    public void State(string pairName, int pairsToAdd, int pairsToRemove, bool expectedHasError, bool expectedIsModified, bool expectedHasItems) {
        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };
        var subject = new NameValuePairModelCollection(collection, Validator.Required);

        subject[0].Name.Value = pairName;

        for (var i = 0; i < pairsToRemove; i++) {
            subject.Remove(subject[^1]);
        }

        for (var i = 0; i < pairsToAdd; i++) {
            subject.Add();
        }

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasItems, subject.HasItems);
    }

    [Fact]
    public void Remove() {
        var collection = new List<NameValuePair>() {
            new() { Name = "", Value = "" },
            new() { Name = "", Value = "" },
            new() { Name = "", Value = "" }
        };

        var subject = new NameValuePairModelCollection(collection, Validator.Required);

        var pair = subject[1];

        subject.Remove(pair);

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Equal(2, subject.Count);
        Assert.DoesNotContain(pair, subject);

        Assert.Equal(3, collection.Count);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Add_Empty(bool isValid, bool expectedHasError) {
        var collection = new List<NameValuePair>();

        var subject = new NameValuePairModelCollection(collection, _ => isValid, _ => isValid);

        subject.Add();

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var pair = Assert.Single(subject);
        Assert.Equal("", pair.Name.Value);
        Assert.Equal(expectedHasError, pair.Name.HasError);
        Assert.Equal("", pair.Value.Value);
        Assert.Equal(expectedHasError, pair.Value.HasError);

        Assert.Empty(collection);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Add_String_Values(bool isValid, bool expectedHasError) {
        const string name = "Name";
        const string value = "Value";

        var collection = new List<NameValuePair>();

        var subject = new NameValuePairModelCollection(collection, _ => isValid, _ => isValid) {
            { name, value }
        };

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var pair = Assert.Single(subject);
        Assert.Equal(name, pair.Name.Value);
        Assert.Equal(expectedHasError, pair.Name.HasError);
        Assert.Equal(value, pair.Value.Value);
        Assert.Equal(expectedHasError, pair.Value.HasError);

        Assert.Empty(collection);
    }

    [Fact]
    public void Set() {
        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" },
            new() { Name = "ThirdName", Value = "ThirdValue" }
        };

        var subject = new NameValuePairModelCollection(collection, Validator.Required);

        foreach (var pair in subject) {
            pair.Name.Value = "ChangedName";
            pair.Value.Value = "ChangedValue";
        }

        subject.Remove(subject[1]);
        subject.Add("NewName", "NewValue");
        subject.Remove(subject[^1]);
        subject.Add("NewName", "NewValue");

        subject.Set();

        Assert.False(subject.IsModified);
        foreach (var pair in subject) {
            Assert.False(pair.IsModified);
        }

        Assert.Equal(3, collection.Count);
        for (var i = 0; i < subject.Count; i++) {
            Assert.Equal(subject[i].Name.Value, collection[i].Name);
            Assert.Equal(subject[i].Value.Value, collection[i].Value);
        }
    }

    [Fact]
    public void Reset() {
        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };

        var subject = new NameValuePairModelCollection(collection, Validator.Required);

        foreach (var pair in subject) {
            pair.Name.Value = "ChangedName";
            pair.Value.Value = "ChangedValue";
        }

        subject.Add("NewName", "NewValue");

        subject.Reset();

        Assert.Equal(2, subject.Count);
        Assert.False(subject.IsModified);
        foreach (var pair in subject) {
            Assert.False(pair.IsModified);
        }
    }
}
