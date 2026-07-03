using System.Collections.Generic;

namespace RequestFiend.Core;

public class Script {
    public List<string> References { get; set; } = [];
    public string Code { get; set; } = "";

    public ScriptSnapshot CreateSnapshot()
        => new([.. References], Code);
}
