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
            Assert.Equal(nameValidator, subject[i].Name.Validator);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
            Assert.Equal(valueValidator, subject[i].Value.Validator);
        }
    }

    [Theory]
    [InlineData(false, false, 0, 0, false, false, true)]
    [InlineData(true, false, 0, 0, true, false, true)]
    [InlineData(false, true, 0, 0, false, true, true)]
    [InlineData(false, false, 1, 0, true, true, true)]
    [InlineData(false, false, 0, 1, false, true, true)]
    [InlineData(false, false, 0, 2, false, true, false)]
    [InlineData(false, false, 1, 2, true, true, true)]
    public void State(bool pairHasEror, bool pairIsModified, int pairsToAdd, int pairsToRemove, bool expectedHasError, bool expectedIsModified, bool expectedHasItems) {
        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };
        var subject = new NameValuePairModelCollection(collection, Validator.Required);

        subject[0].HasError = pairHasEror;
        subject[0].IsModified = pairIsModified;

        for (var i = 0; i < pairsToRemove; i++) {
            subject.Remove(subject[^1]);
        }

        for (var i = 0; i < pairsToAdd; i++) {
            subject.Add(new(() => "", () => "", Validator.Required));
        }

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasItems, subject.HasItems);
    }

    [Fact]
    public void Remove() {
        var subject = new NameValuePairModelCollection([
            new() { Name = "", Value = "" },
            new() { Name = "", Value = "" },
            new() { Name = "", Value = "" }
        ], Validator.Required);

        var pair = subject[1];

        subject.Remove(pair);

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Equal(2, subject.Count);
        Assert.DoesNotContain(pair, subject);
    }

    [Fact]
    public void Add_Empty() {
        var nameValidator = (string value) => true;
        var valueValidator = (string value) => true;

        var subject = new NameValuePairModelCollection([], nameValidator, valueValidator);

        subject.Add();

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var pair = Assert.Single(subject);
        Assert.Equal("", pair.Name.Value);
        Assert.Equal(nameValidator, pair.Name.Validator);
        Assert.Equal("", pair.Value.Value);
        Assert.Equal(valueValidator, pair.Value.Validator);
    }

    [Fact]
    public void Add_String_Values() {
        const string name = "Name";
        const string value = "Value";

        var nameValidator = (string value) => true;
        var valueValidator = (string value) => true;

        var subject = new NameValuePairModelCollection([], nameValidator, valueValidator) {
            { name, value }
        };

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var pair = Assert.Single(subject);
        Assert.Equal(name, pair.Name.Value);
        Assert.Equal(nameValidator, pair.Name.Validator);
        Assert.Equal(value, pair.Value.Value);
        Assert.Equal(valueValidator, pair.Value.Validator);
    }

    [Fact]
    public void Reset() {
        var subject = new NameValuePairModelCollection([
            new() { Name = "", Value = "" }
        ], Validator.Required);

        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };

        subject.Add(new(() => "", () => "", Validator.Required));

        subject.Reset(collection);

        Assert.False(subject.IsModified);
        Assert.Equal(collection.Count, subject.Count);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
        }
    }

    [Fact]
    public void GetNameValuePairs() {
        var subject = new NameValuePairModelCollection([
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        ], Validator.Required);

        var result = subject.GetNameValuePairs();

        Assert.Equal(subject.Count, result.Count);

        for (var i = 0; i < subject.Count; i++) {
            Assert.Equal(subject[i].Name.Value, result[i].Name);
            Assert.Equal(subject[i].Value.Value, result[i].Value);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void Reset_Throws_For_Wrong_Length(int collectionLength) {
        var subject = new NameValuePairModelCollection([
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        ], Validator.Required);

        var collection = Enumerable.Range(0, collectionLength)
            .Select(_ => new NameValuePair() { Name = "Name" })
            .ToList();

        Assert.Throws<ArgumentException>(() => subject.Reset(collection));
    }
}
