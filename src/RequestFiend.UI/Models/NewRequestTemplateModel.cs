using RequestFiend.Core;
using System.Collections.Generic;

namespace RequestFiend.UI.Models;

public class NewRequestTemplateModel {
    public List<string> Methods { get; set; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];
    public required RequestTemplateCollection Collection { get; init; }
    public string? Name { get; set; }
    public string? Method { get; set; }
    public string? Url { get; set; }
}
