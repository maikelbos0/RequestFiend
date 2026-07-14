using RequestFiend.Core;

namespace RequestFiend.Models;

public record RequestTemplateItemModel(RequestTemplate Request) : IImmutable;
