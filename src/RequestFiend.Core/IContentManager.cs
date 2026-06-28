using System;
using System.Net.Http;

namespace RequestFiend.Core;

public interface IContentManager {
    [Obsolete]
    HttpContent? GetContent(RequestTemplate request, VariableSnapshot collection);
    HttpContent? GetContent(RequestTemplateSnapshot request);
}
