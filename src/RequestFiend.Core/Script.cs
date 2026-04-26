namespace RequestFiend.Core;

public class Script {
    public string Code { get; set; } = "";

    public Script Clone()
        => new() {
            Code = Code
        };
}
