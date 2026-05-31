using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RequestFiend.Core.Tests;

public class FormDataContentManagerTests {
    [Theory]
    [InlineData(false, "multipart/form-data")]
    [InlineData(true, null)]
    public async Task GetContent(bool hasManualContentTypeHeader, string? expectedMediaType) {
        var subject = new FormDataContentManager();
        var request = new RequestTemplate() {

            Name = "Request",
            Method = "POST",
            Url = "https://localhost",
            HasManualContentTypeHeader = hasManualContentTypeHeader,
            FormFieldContent = [
                new() { Name = "Description", Value = "The {{first}} and {{second}} get replaced" }
            ],
            FormFileContent = [
                new() { Name = "Data", Value = "./{{FileName}}" }
            ]
        };
        var collection = new RequestTemplateCollection() {
            Variables = {
                new() { Name = "FileName", Value = "Data.json" },
                new() { Name = "First", Value = "Replacement" },
                new() { Name =  "Second", Value = "Another" }
            }
        };

        var result = Assert.IsType<MultipartFormDataContent>(subject.GetContent(request, collection));

        Assert.Equal(expectedMediaType, result.Headers.ContentType?.MediaType);
        Assert.Equal("The Replacement and Another get replaced", await Assert.IsType<StringContent>(Assert.Single(result, content => content.Headers.ContentDisposition?.Name == "Description")).ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal(File.ReadAllBytes("./Data.json"), await Assert.IsType<ByteArrayContent>(Assert.Single(result, content => content.Headers.ContentDisposition?.Name == "Data")).ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
