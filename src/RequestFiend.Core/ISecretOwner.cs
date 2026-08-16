namespace RequestFiend.Core;

public interface ISecretOwner {
    byte[]? Salt { get; set; }
}
