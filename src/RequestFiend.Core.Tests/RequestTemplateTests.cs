using System;
using System.Net.Http;
using Xunit;

namespace RequestFiend.Core.Tests;

public class RequestTemplateTests {
    [Fact]
    public void Clone() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            Headers = {
                new() { Name = "Accept", Value = "application/json" }
            },
            ContentType = ContentType.Text,
            StringContent = "Just a piece of text",
            PreExchangeScript = { Code = "PreExchangeScript" },
            PostExchangeScript = { Code = "PostExchangeScript" },
            OnExceptionScript = { Code = "OnExceptionScript" }
        };

        var result = subject.Clone();

        Assert.Equal(subject.Name, result.Name);
        Assert.Equal(subject.Method, result.Method);
        Assert.Equal(subject.Url, result.Url);
        Assert.NotSame(subject.Headers, result.Headers);
        Assert.Equal(subject.Headers.Count, result.Headers.Count);
        Assert.Equal(subject.ContentType, result.ContentType);
        Assert.Equal(subject.StringContent, result.StringContent);
        Assert.NotSame(subject.PreExchangeScript, result.PreExchangeScript);
        Assert.Equal(subject.PreExchangeScript.Code, result.PreExchangeScript.Code);
        Assert.NotSame(subject.PostExchangeScript, result.PostExchangeScript);
        Assert.Equal(subject.PostExchangeScript.Code, result.PostExchangeScript.Code);
        Assert.NotSame(subject.OnExceptionScript, result.OnExceptionScript);
        Assert.Equal(subject.OnExceptionScript.Code, result.OnExceptionScript.Code);
    }

    [Fact]
    public void CreateSnapshot() {
        var subject = new RequestTemplate() {
            Name = "Request",
            Method = "GET",
            Url = "https://localhost:7001/",
            Headers = {
                new() { Name = "Accept", Value = "application/json" }
            },
            ContentType = ContentType.Text,
            HasManualContentTypeHeader = true,
            StringContent = "Just a piece of text",
            FileContent = "File",
            FormFieldContent = {
                new() { Name = "Value", Value = "Some value" }
            },
            FormFileContent = {
                new() { Name = "File", Value = @"C:\Documents\Data.json" }
            },
            PreExchangeScript = { Code = "PreExchangeScript" },
            PostExchangeScript = { Code = "PostExchangeScript" },
            OnExceptionScript = { Code = "OnExceptionScript" }
        };
        var collection = new RequestTemplateCollection() {
            Requests = { subject },
            DefaultHeaders = {
                new() { Name = "User-agent", Value = "RequestFiend" }
            },
            Variables = {
                new() { Name = "Foo", Value = "Value" }
            }
        };
        var environment = new Environment() {
            Variables = {
                new() { Name = "Bar", Value = "Value" }
            }
        };

        var result = subject.CreateSnapshot(collection, environment);

        Assert.Equal(collection.Variables.Count + environment.Variables.Count, result.Variables.Variables.Count);
        Assert.Equal(subject.Name, result.Name);
        Assert.Equal(subject.Method, result.Method);
        Assert.Equal(subject.Url, result.Url);
        Assert.Equal(collection.DefaultHeaders.Count + subject.Headers.Count, result.Headers.Length);
        Assert.Equal(subject.ContentType, result.ContentType);
        Assert.Equal(subject.HasManualContentTypeHeader, result.HasManualContentTypeHeader);
        Assert.Equal(subject.StringContent, result.StringContent);
        Assert.Equal(subject.FileContent, result.FileContent);
        Assert.Equal(subject.FormFieldContent.Count, result.FormFieldContent.Length);
        Assert.Equal(subject.FormFileContent.Count, result.FormFileContent.Length);
        Assert.Equal(subject.PreExchangeScript.Code, result.PreExchangeScript.Code);
        Assert.Equal(subject.PostExchangeScript.Code, result.PostExchangeScript.Code);
        Assert.Equal(subject.OnExceptionScript.Code, result.OnExceptionScript.Code);
    }
}
