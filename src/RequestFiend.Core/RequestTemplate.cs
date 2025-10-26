namespace RequestFiend.Core;

public class RequestTemplate
{
    public required string Name { get; set; }
    public required HttpMethod Method { get; set; }
    public required string Url { get; set; }
    public List<HeaderTemplate> HeaderTemplates { get; set; } = [];
}
