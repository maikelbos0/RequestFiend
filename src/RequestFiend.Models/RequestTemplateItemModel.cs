using RequestFiend.Core;

namespace RequestFiend.Models;

// TODO make request private somehow?
public record RequestTemplateItemModel(RequestTemplate Request) : IImmutable;
