using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Internal;

namespace RequestFiend.Models.Tests;

public class NameValuePairModelCollectionTests {
    [Theory]
    [InlineData("", true)]
    [InlineData("FirstName", false)]
    public void Constructor(string name, bool expectedHasError) {
        var collection = new List<NameValuePair>() {
            new() { Name = name, Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };
        var subject = new NameValuePairModelCollection(collection, Validator.Required);

        Assert.Equal(collection.Count, subject.Count);
        Assert.Equal(expectedHasError, subject.HasError);
        Assert.False(subject.IsModified);
        Assert.True(subject.HasItems);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
            Assert.Equal(collection[i].Value, subject[i].Value.Value);
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
            subject.Add(new(Validator.Required));
        }

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasItems, subject.HasItems);
    }

    [Fact]
    public void OnAddClicked() {
        var subject = new NameValuePairModelCollection([], Validator.Required);

        subject.OnAddClicked();

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Single(subject);
    }

    [Fact]
    public void OnRemoveClicked() {
        var pair = new NameValuePairModel(Validator.Required);
        var subject = new NameValuePairModelCollection([], Validator.Required) {
            new(Validator.Required),
            pair,
            new(Validator.Required)
        };

        subject.OnRemoveClicked(pair);

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Equal(2, subject.Count);
        Assert.DoesNotContain(pair, subject);
    }

    [Fact]
    public void Add() {
        var subject = new NameValuePairModelCollection([], Validator.Variable);

        subject.Add("Variable name", "Variable value");

        Assert.True(subject.HasError);

        var pair = Assert.Single(subject);
        Assert.Equal("Variable name", pair.Name.Value);
        Assert.Equal("Variable value", pair.Value.Value);
    }

    [Fact]
    public void Reset() {
        var subject = new NameValuePairModelCollection([], Validator.Required) {
            new(Validator.Required)
        };

        var collection = new List<NameValuePair>() {
            new() { Name = "FirstName", Value = "FirstValue" },
            new() { Name = "SecondName", Value = "SecondValue" }
        };

        subject.Add(new(Validator.Required));

        subject.Reset(collection);

        Assert.False(subject.IsModified);
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
    public void Reset_Throws_For_Wrong_Length(int collectionLength) {
        var subject = new NameValuePairModelCollection([], Validator.Required) {
            new(Validator.Required),
            new(Validator.Required)
        };

        var collection = Enumerable.Range(0, collectionLength)
            .Select(_ => new NameValuePair() { Name = "Name" })
            .ToList();

        Assert.Throws<ArgumentException>(() => subject.Reset(collection));
    }
}
