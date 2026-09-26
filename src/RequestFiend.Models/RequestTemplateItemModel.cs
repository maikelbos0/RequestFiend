using RequestFiend.Core;

namespace RequestFiend.Models;

public record RequestTemplateItemModel : IImmutable {
    private readonly RequestTemplate request;

    public string Name => request.Name;

    public RequestTemplateItemModel(RequestTemplate request) {
        this.request = request;
    }

    public bool Equals(RequestTemplate request)
        => this.request == request;
}
