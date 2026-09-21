using RequestFiend.Core;
using System;

namespace RequestFiend.Models.Messages;

public record RequestTemplateCollectionSettingsUpdatedMessage(FileModel File, [property: Obsolete] RequestTemplateCollection Collection);
