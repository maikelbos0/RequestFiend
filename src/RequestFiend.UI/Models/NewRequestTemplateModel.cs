using System.Net.Http;

namespace RequestFiend.UI.Models;

public class NewRequestTemplateModel {
    public string Name { get; set; } = "New request";
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string Url { get; set; } = "https://localhost/";
}
