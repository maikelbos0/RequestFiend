using Microsoft.Extensions.DependencyInjection;

namespace RequestFiend.Core;

public class Secret {
    public required string Name { get; set; }
    public string? Ciphertext { get; set; }

    public string GetValue(ISecretOwner owner) {
        if (Ciphertext == null) {
            return "";
        }

        return AppHost.Services.GetRequiredService<ISecretEncryptor>().Decrypt(owner, Ciphertext);
    }

    public void SetValue(ISecretOwner owner, string? value) {
        if (value == null) {
            Ciphertext = null;
        }
        else {
            Ciphertext = AppHost.Services.GetRequiredService<ISecretEncryptor>().Encrypt(owner, value);
        }
    }
}
