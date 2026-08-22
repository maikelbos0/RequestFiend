using System.Collections.Generic;

namespace RequestFiend.Core;

public class Environment : ISecretOwner {
    public List<NameValuePair> Variables { get; set; } = [];
    public List<Secret> Secrets { get; set; } = [];
    public string? EncryptionData { get; set; }
}
