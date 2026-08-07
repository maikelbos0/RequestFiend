namespace RequestFiend.Core;

public interface ISecretOwner {
    public byte[]? Salt { get; set; }
}
