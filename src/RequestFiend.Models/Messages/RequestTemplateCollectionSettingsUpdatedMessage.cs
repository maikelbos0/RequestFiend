using RequestFiend.Core;
using System;

namespace RequestFiend.Models.Messages;

public record RequestTemplateCollectionSettingsUpdatedMessage(string FilePath, [property: Obsolete] RequestTemplateCollection Collection);
