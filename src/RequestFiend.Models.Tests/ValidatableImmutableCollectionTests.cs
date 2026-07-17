using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace RequestFiend.Models.Tests;

public class ValidatableImmutableCollectionTests {
    [Fact]
    public void Constructor() {
        var collection = new List<IImmutable>() {
            Substitute.For<IImmutable>(),
            Substitute.For<IImmutable>()
        };
        var subject = new ValidatableImmutableCollection<IImmutable>(() => collection, Substitute.For<Action<IEnumerable<IImmutable>>>());

        Assert.Equal(collection, subject);
        Assert.False(subject.IsModified);
        Assert.True(subject.HasItems);
    }

    [Theory]
    [InlineData(0, 0, false, true)]
    [InlineData(1, 0, true, true)]
    [InlineData(0, 1, true, true)]
    [InlineData(0, 2, true, false)]
    [InlineData(1, 2, true, true)]
    public void State(int valuesToAdd, int valuesToRemove, bool expectedIsModified, bool expectedHasItems) {
        var subject = new ValidatableImmutableCollection<IImmutable>(() => [
            Substitute.For<IImmutable>(),
            Substitute.For<IImmutable>()
        ], Substitute.For<Action<IEnumerable<IImmutable>>>());

        for (var i = 0; i < valuesToRemove; i++) {
            subject.Remove(subject[^1]);
        }

        for (var i = 0; i < valuesToAdd; i++) {
            subject.Add(Substitute.For<IImmutable>());
        }

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasItems, subject.HasItems);
    }

    [Fact]
    public void Remove() {
        var subject = new ValidatableImmutableCollection<IImmutable>(() => [
            Substitute.For<IImmutable>(),
            Substitute.For<IImmutable>(),
            Substitute.For<IImmutable>()
        ], Substitute.For<Action<IEnumerable<IImmutable>>>());

        var fileModel = subject[1];

        subject.Remove(fileModel);

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Equal(2, subject.Count);
    }

    [Fact]
    public void Add() {
        var subject = new ValidatableImmutableCollection<IImmutable>(() => [], Substitute.For<Action<IEnumerable<IImmutable>>>()) {
            Substitute.For<IImmutable>()
        };

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
    }

    [Fact]
    public void Set() {
        var setter = Substitute.For<Action<IEnumerable<IImmutable>>>();

        var subject = new ValidatableImmutableCollection<IImmutable>(() => [], setter) {
            Substitute.For<IImmutable>()
        };

        subject.Set();

        setter.Received(1).Invoke(subject);
        Assert.False(subject.IsModified);
    }

    [Fact]
    public void Reset() {
        var subject = new ValidatableImmutableCollection<IImmutable>(() => [Substitute.For<IImmutable>()], Substitute.For<Action<IEnumerable<IImmutable>>>()) {
            Substitute.For<IImmutable>()
        };

        subject.Reset();

        Assert.Single(subject);
        Assert.True(subject.HasItems);
        Assert.False(subject.IsModified);
    }
}
