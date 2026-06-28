using System.Collections.Immutable;

namespace RequestFiend.Core;

public record ScriptSnapshot(ImmutableArray<string> References, string Code);
