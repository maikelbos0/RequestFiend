namespace RequestFiend.Core;

public interface IPasswordProvider {
    string Provide(ISecretOwner owner);
}
