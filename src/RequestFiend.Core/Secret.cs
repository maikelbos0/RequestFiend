using Microsoft.Extensions.DependencyInjection;

namespace RequestFiend.Core;

public class Secret {
    public required string Name { get; set; }
    public string? EncryptedValue { get; set; }

    public string GetPlaintextValue(ISecretOwner owner) {
        if (EncryptedValue == null) {
            return "";
        }
        else if (AppHost.Services.GetRequiredService<ISecretEncryptor>().TryDecrypt(owner, EncryptedValue, out var result)) {
            return result;
        }
        else {
            return "********";
        }
    }

    public void SetPlaintextValue(ISecretOwner owner, string? value) {
        if (value == null) {
            EncryptedValue = null;
        }
        else if (AppHost.Services.GetRequiredService<ISecretEncryptor>().TryEncrypt(owner, value, out var result)) {
            EncryptedValue = result;
        }
    }
}
