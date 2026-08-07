using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;

namespace RequestFiend.Core;

public static class ContentManagerProvider {
    public static IContentManager Provide(ContentType contentType) => contentType switch {
        ContentType.None => new NoneContentManager(),
        ContentType.Text => new TextContentManager(),
        ContentType.Json => new JsonContentManager(),
        ContentType.Xml => new XmlContentManager(),
        ContentType.File => new FileContentManager(AppHost.Services.GetRequiredService<IFileSystem>()),
        ContentType.FormData => new FormDataContentManager(AppHost.Services.GetRequiredService<IFileSystem>()),
        _ => throw new NotImplementedException($"Received unknown content type '{contentType}'.")
    };
}

