using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
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
        References = new(
            () => string.Join(System.Environment.NewLine, script.References),
            value => script.References = [.. GetNewLineFinder().Split(value)
                .Where(reference => !string.IsNullOrWhiteSpace(reference))
                .Select(reference => reference.Trim())]
        );
        Code = new(() => script.Code, value => script.Code = value);

        ConfigureState([References, Code]);
    }

    [RelayCommand]
    public void ToggleShowReferences()
        => ShowReferences = !ShowReferences;
}
