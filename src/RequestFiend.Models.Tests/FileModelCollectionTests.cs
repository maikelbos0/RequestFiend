using System.Collections.Generic;
using Xunit;

namespace RequestFiend.Models.Tests;

public class FileModelCollectionTests {
    [Fact]
    public void Constructor() {
        var collection = new List<FileModel>() {
            new(@"C:\Documents\Foo.json"),
            new(@"C:\Documents\Bar.json")
        };
        var subject = new FileModelCollection(collection);

        Assert.Equal(collection.Count, subject.Count);
        Assert.False(subject.IsModified);
        Assert.True(subject.HasItems);

        for (var i = 0; i < collection.Count; i++) {
            Assert.Equal(collection[i], subject[i]);
        }
    }

    [Theory]
    [InlineData(0, 0, false, true)]
    [InlineData(1, 0, true, true)]
    [InlineData(0, 1, true, true)]
    [InlineData(0, 2, true, false)]
    [InlineData(1, 2, true, true)]
    public void State(int modelsToAdd, int modelsToRemove, bool expectedIsModified, bool expectedHasItems) {
        var collection = new List<FileModel>() {
            new(@"C:\Documents\Foo.json"),
            new(@"C:\Documents\Bar.json")
        };
        var subject = new FileModelCollection(collection);

        for (var i = 0; i < modelsToRemove; i++) {
            subject.Remove(subject[^1]);
        }

        for (var i = 0; i < modelsToAdd; i++) {
            subject.Add(new(@"C:\Documents\Baz.json"));
        }

        Assert.Equal(expectedIsModified, subject.IsModified);
        Assert.Equal(expectedHasItems, subject.HasItems);
    }

    [Fact]
    public void Remove() {
        var subject = new FileModelCollection([
            new(@"C:\Documents\Foo.json"),
            new(@"C:\Documents\Bar.json"),
            new(@"C:\Documents\Baz.json")
        ]);

        var fileModel = subject[1];

        subject.Remove(fileModel);

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
        Assert.Equal(2, subject.Count);
    }

    [Fact]
    public void Add() {
        var subject = new FileModelCollection([]) {
            new(@"C:\Documents\Foo.json")
        };

        Assert.True(subject.IsModified);
        Assert.True(subject.HasItems);
    }
}
