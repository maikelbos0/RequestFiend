using System;

namespace RequestFiend.Models.Services;

public interface IMessageService {
    void Send<TMessage>(TMessage message) where TMessage : class;
    void Send<TMessage, TToken>(TMessage message, TToken token) where TMessage : class where TToken : IEquatable<TToken>;
}
