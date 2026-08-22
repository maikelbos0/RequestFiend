namespace RequestFiend.Core;

public interface ISecretOwner {
    string? EncryptionData { get; set; }
}
