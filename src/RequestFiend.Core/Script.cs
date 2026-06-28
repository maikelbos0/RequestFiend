using System;
using System.Collections.Generic;

namespace RequestFiend.Core;

public class Script {
    public List<string> References { get; set; } = [];
    public string Code { get; set; } = "";

    [Obsolete]
    public Script Clone()
        => new() {
            Code = Code
        };

    public ScriptSnapshot CreateSnapshot()
        => new([.. References], Code);
}
