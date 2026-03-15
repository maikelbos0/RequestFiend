using System.Collections.Generic;
using Xunit;

namespace RequestFiend.Models.Tests;

public class HttpHeaderModelTests {
    [Fact]
    public void Constructor() {
        var header = new KeyValuePair<string, IEnumerable<string>>("Accept", ["application/json", "application/xml"]);

        var subject = new HttpHeaderModel(header);

        Assert.Equal(header.Key, subject.Name);
        Assert.Equal(string.Join(", ", header.Value), subject.Value);
    }
}
