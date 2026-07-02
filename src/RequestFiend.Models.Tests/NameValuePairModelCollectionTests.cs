using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelCollectionTests {
    [Fact]
    public void Constructor() {
        var nameValidator = (string value) => true;
        var valueValidator = (string value) => true;

        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };
        var subject = new NameValuePairModelCollection(collection, nameValidator, valueValidator);

        Assert.Equal(collection.Count, subject.Count);
        Assert.False(subject.IsModified);
        Assert.True(subject.HasItems);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Same(nameValidator, subject[i].Name.Validator);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
            Assert.Same(valueValidator, subject[i].Value.Validator);
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
        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void Add_Empty() {
        var nameValidator = (string value) => true;
        var valueValidator = (string value) => true;

        var collection = new List<NameValuePair>();

        var subject = new NameValuePairModelCollection(collection, nameValidator, valueValidator);

        subject.Add();

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var pair = Assert.Single(subject);
        Assert.Equal("", pair.Name.Value);
        Assert.Same(nameValidator, pair.Name.Validator);
        Assert.Equal("", pair.Value.Value);
        Assert.Same(valueValidator, pair.Value.Validator);
        Assert.Single(collection);
    }

    [Fact]
    public void Add_String_Values() {
        const string name = "Name";
        const string value = "Value";

        var nameValidator = (string value) => true;
        var valueValidator = (string value) => true;

        var collection = new List<NameValuePair>();

        var subject = new NameValuePairModelCollection(collection, nameValidator, valueValidator) {
            { name, value }
        };

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var pair = Assert.Single(subject);
        Assert.Equal(name, pair.Name.Value);
        Assert.Same(nameValidator, pair.Name.Validator);
        Assert.Equal(value, pair.Value.Value);
        Assert.Same(valueValidator, pair.Value.Validator);
        Assert.Single(collection);
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

        subject.Reset();

        Assert.False(subject.IsModified);
        foreach (var pair in subject) {
            Assert.False(pair.IsModified);
        }
    }
}
