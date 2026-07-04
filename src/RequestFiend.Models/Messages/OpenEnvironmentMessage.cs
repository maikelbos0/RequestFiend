using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record OpenEnvironmentMessage(FileModel File, Environment Environment);