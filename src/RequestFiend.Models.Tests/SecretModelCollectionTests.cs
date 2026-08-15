using NSubstitute;
using RequestFiend.Core;
using System.Collections.Generic;
using Xunit;

namespace RequestFiend.Models.Tests;

public class SecretModelCollectionTests : TestsBase {
    [Fact]
    public void Constructor() {
        var collection = new List<Secret>() {
            new() { Name = "FirstName" },
            new() { Name = "SecondName" }
        };
        var subject = new SecretModelCollection(Substitute.For<ISecretOwner>(), collection);

        Assert.Equal(collection.Count, subject.Count);
        Assert.False(subject.IsModified);
        Assert.True(subject.HasItems);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i].Name, subject[i].Name.Value);
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
    public void State(string secretName, int secretsToAdd, int secretsToRemove, bool expectedHasError, bool expectedIsModified, bool expectedHasItems) {
        var collection = new List<Secret>() {
            new() { Name = "FirstName" },
            new() { Name = "SecondName" }
        };
        var subject = new SecretModelCollection(Substitute.For<ISecretOwner>(), collection);

        subject[0].Name.Value = secretName;

        for (var i = 0; i < secretsToRemove; i++) {
            subject.Remove(subject[^1]);
        }

        for (var i = 0; i < secretsToAdd; i++) {
            subject.Add();
        }

        Assert.Equal(expectedHasError, subject.HasError);
        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasItems, subject.HasItems);
    }

    [Fact]
    public void Remove() {
        var collection = new List<Secret>() {
            new() { Name = "" },
            new() { Name = "" },
            new() { Name = "" }
        };

        var subject = new SecretModelCollection(Substitute.For<ISecretOwner>(), collection);

        var secret = subject[1];

        subject.Remove(secret);

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Equal(2, subject.Count);
        Assert.DoesNotContain(secret, subject);

        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void Add_Empty() {
        var collection = new List<Secret>();

        var subject = new SecretModelCollection(Substitute.For<ISecretOwner>(), collection);

        subject.Add();

        Assert.True(subject.HasError);
        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);

        var secret = Assert.Single(subject);
        Assert.Equal("", secret.Name.Value);
        Assert.Equal("", secret.Value.Value);

        Assert.Empty(collection);
    }

    [Fact]
    public void Set() {
        var collection = new List<Secret>() {
            new() { Name = "FirstName" },
            new() { Name = "SecondName" },
            new() { Name = "ThirdName" }
        };
        var subject = new SecretModelCollection(Substitute.For<ISecretOwner>(), collection);
        
        foreach (var secret in subject) {
            secret.Name.Value = "ChangedName";
        }

        subject.Remove(subject[1]);
        subject.Add(new Secret() { Name = "NewName" });
        subject.Remove(subject[^1]);
        subject.Add(new Secret() { Name = "NewName" });

        subject.Set();

        Assert.False(subject.IsModified);
        foreach (var secret in subject) {
            Assert.False(secret.IsModified);
        }

        Assert.Equal(3, collection.Count);
        for (var i = 0; i < subject.Count; i++) {
            Assert.Equal(subject[i].Name.Value, collection[i].Name);
        }
    }

    [Fact]
    public void Reset() {
        var collection = new List<Secret>() {
            new() { Name = "FirstName" },
            new() { Name = "SecondName" }
        };
        var subject = new SecretModelCollection(Substitute.For<ISecretOwner>(), collection);

        foreach (var secret in subject) {
            secret.Name.Value = "ChangedName";
        }

        subject.Add(new Secret() { Name = "NewName" });

        subject.Reset();

        Assert.Equal(2, subject.Count);
        Assert.False(subject.IsModified);
        foreach (var secret in subject) {
            Assert.False(secret.IsModified);
        }
    }
}
