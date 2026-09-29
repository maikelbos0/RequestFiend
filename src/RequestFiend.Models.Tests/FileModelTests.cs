using Xunit;

namespace RequestFiend.Models.Tests;

public class FileModelTests : TestsBase {
    [Fact]
    public void OperatorString() {
        const string filePath = @"C:\Documents\External data requests.json";

        string result = new FileModel(filePath);

        Assert.Equal(filePath, result);
    }

    [Fact]
    public void OperatorFileModel() {
        const string filePath = @"C:\Documents\External data requests.json";

        FileModel result = filePath;

        Assert.Equal(filePath, result.FilePath);
    }
}
