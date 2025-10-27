namespace RequestFiend.Core;

public interface IContentTemplate {
    string MediaType { get; }
    string? CharSet { get; }
    bool Validate(RequestTemplateCollection collection);
    bool Format(RequestTemplateCollection collection);
    byte[] GetContent(RequestTemplateCollection collection);
}
