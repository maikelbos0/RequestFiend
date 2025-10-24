namespace RequestFiend.Core;

public class RequestTemplateCollection {
    public string? Name { get; set; }
    public string? BaseUrl { get; set; }
    public List<RequestTemplate> RequestTemplates { get; set; } = [];
}
