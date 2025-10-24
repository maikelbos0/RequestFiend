namespace RequestFiend.Core;

public class RequestTemplate
{
    public string? Name { get; set; }
    public HttpMethod? Method { get; set; }
    public string? Url { get; set; }
}
