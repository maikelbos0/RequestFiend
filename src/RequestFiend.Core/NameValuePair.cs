namespace RequestFiend.Core;

public class NameValuePair {
    public required string Name { get; set; }
    public string Value { get; set; } = "";

    public NameValuePair Clone()
        => new() { 
            Name = Name, 
            Value = Value 
        };

    public NameValuePairSnapshot CreateSnapshot()
        => new(Name, Value);
}
