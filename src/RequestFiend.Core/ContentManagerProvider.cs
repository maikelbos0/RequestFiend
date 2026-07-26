using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace RequestFiend.Core;

public static class ContentManagerProvider {
    private static Dictionary<ContentType, IContentManager>? contentManagers;

    public static void Initialize(IFileSystem fileSystem) {
        contentManagers = new() {
            { ContentType.None, new NoneContentManager() },
            { ContentType.Text, new TextContentManager() },
            { ContentType.Json, new JsonContentManager() },
            { ContentType.Xml, new XmlContentManager() },
            { ContentType.File, new FileContentManager(fileSystem) },
            { ContentType.FormData, new FormDataContentManager(fileSystem) }
        };
    }

    public static IContentManager Provide(ContentType contentType) {
        if (contentManagers == null) {
            throw new InvalidOperationException($"{nameof(Initialize)} must be called before calling {nameof(Provide)}.");
        }

        if (contentManagers.TryGetValue(contentType, out var contentManager)) {
            return contentManager;
        }

        throw new NotImplementedException($"Received unknown content type '{contentType}'.");
    }
}
