using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public partial class ScriptModel : BoundModelBase, IValidatable {
    public ValidatableProperty<string> Code { get; }

    public ScriptModel(Script script) {
        Code = new(() => script.Code);

        ConfigureState([Code]);
    }

    public void Update(Script script) {
        script.Code = Code.Value;
    }

    public void Reset() {
        Code.Reset();
    }
}
