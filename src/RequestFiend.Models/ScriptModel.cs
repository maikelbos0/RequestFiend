using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RequestFiend.Models;

public partial class ScriptModel : BoundModelBase, IValidatable {

    [GeneratedRegex(@"[\r\n]+", RegexOptions.Compiled)]
    private static partial Regex GetNewLineFinder();

    [ObservableProperty] public partial bool ShowReferences { get; set; }
    public ValidatableProperty<string> References { get; }
    public ValidatableProperty<string> Code { get; }

    public ScriptModel(Script script) {
        ShowReferences = script.References.Count > 0;
        References = new(() => string.Join(Environment.NewLine, script.References));
        Code = new(() => script.Code);

        ConfigureState([References, Code]);
    }

    public void Update(Script script) {
        script.References = [.. GetNewLineFinder().Split(References.Value)
            .Where(reference => !string.IsNullOrWhiteSpace(reference))
            .Select(reference => reference.Trim())];
        script.Code = Code.Value;
    }

    public void Reset() {
        References.Reset();
        Code.Reset();
    }

    [RelayCommand]
    public void ToggleShowReferences()
        => ShowReferences = !ShowReferences;
}
