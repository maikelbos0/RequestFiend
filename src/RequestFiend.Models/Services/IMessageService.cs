using CommunityToolkit.Mvvm.Messaging;
using System;

namespace RequestFiend.Models.Services;

public interface IMessageService {
    void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> messageHandler) where TRecipient : class where TMessage : class where TToken : IEquatable<TToken>;
    void Send<TMessage>(TMessage message) where TMessage : class;
    void Send<TMessage, TToken>(TMessage message, TToken token) where TMessage : class where TToken : IEquatable<TToken>;
}
