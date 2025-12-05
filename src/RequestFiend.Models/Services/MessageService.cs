using CommunityToolkit.Mvvm.Messaging;
using System;

namespace RequestFiend.Models.Services;

public class MessageService : IMessageService {
    public void Send<TMessage>(TMessage message) where TMessage : class
        => WeakReferenceMessenger.Default.Send(message);

    public void Send<TMessage, TToken>(TMessage message, TToken token) where TMessage : class where TToken : IEquatable<TToken>
        => WeakReferenceMessenger.Default.Send(message, token);
}