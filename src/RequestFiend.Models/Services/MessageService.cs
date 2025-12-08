using CommunityToolkit.Mvvm.Messaging;
using System;

namespace RequestFiend.Models.Services;

public class MessageService : IMessageService {
    public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> messageHandler) where TRecipient : class where TMessage : class where TToken : IEquatable<TToken>
        => WeakReferenceMessenger.Default.Register(recipient, token, messageHandler);

    public void Send<TMessage>(TMessage message) where TMessage : class
        => WeakReferenceMessenger.Default.Send(message);

    public void Send<TMessage, TToken>(TMessage message, TToken token) where TMessage : class where TToken : IEquatable<TToken>
        => WeakReferenceMessenger.Default.Send(message, token);
}