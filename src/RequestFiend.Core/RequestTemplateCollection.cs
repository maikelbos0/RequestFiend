using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Core;

public class RequestTemplateCollection {
    private readonly Dictionary<string, object> sessionData = [];
    private readonly Dictionary<string, string> sessionVariables = [];

    public string DefaultUrl { get; set; } = "";
    public bool IgnoreRemoteCertificateNotAvailable { get; set; }
    public bool IgnoreRemoteCertificateNameMismatch { get; set; }
    public bool IgnoreRemoteCertificateChainErrors { get; set; }
    public List<RequestTemplate> Requests { get; set; } = [];
    public List<NameValuePair> Variables { get; set; } = [];
    public List<NameValuePair> DefaultHeaders { get; set; } = [];

    public Dictionary<string, object> GetSessionData() => sessionData;

    public Dictionary<string, string> GetSessionVariables() => sessionVariables;

    public VariableSnapshot GetVariableSnapshot(Environment? environment)
        => new(sessionVariables.Select(pair => new NameValuePair() { Name = pair.Key, Value = pair.Value }), Variables, environment?.Variables ?? []);
}
