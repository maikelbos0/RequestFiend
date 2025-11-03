using System.Collections.Generic;

namespace RequestFiend.UI.Models;

public class NewRequestTemplateModel {
    public List<string> Methods { get; set; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];

    public string Name { get; set; } = "New request";
    public string Method { get; set; } = "POST";
    public string Url { get; set; } = "https://localhost/";
}
