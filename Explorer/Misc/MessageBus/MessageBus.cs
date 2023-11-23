using Explorer.Interfaces.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Explorer.Misc.MessageBus;

public class MessageBus : IMessageBus
{
    private readonly Dictionary<Type, List<object>> _observers = new();
    private bool _isDisposed;

    public async Task SendAsync<T>(T message) where T : IMessage
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var messageType = typeof(T);
        if (!_observers.ContainsKey(messageType))
            return;

        var subscriptions = _observers[messageType];
        if (subscriptions == null || subscriptions.Count == 0) 
            return;
        
        foreach (var handler in subscriptions
                                .OfType<ISubscription<T>>()
                                .Select(s => s!.Handler))
        {
            if (handler is { })
                await handler.Invoke(message);
        }
    }

    public IDisposable Subscribe<T>(Func<T, Task> callBack) where T : IMessage
    {
        var messageType = typeof(T);

        ref var subscriptions = ref CollectionsMarshal.GetValueRefOrAddDefault(_observers, messageType, out bool exists)!;
        if (!exists)
        {
            subscriptions = new List<object>();
        }

        var existingSubscription = subscriptions
                                   .Cast<ISubscription<T>>()
                                   .FirstOrDefault(s => s!.Handler == callBack);

        if (existingSubscription is null)
        {
            Subscription<T>? subscription = null;

            var d = Disposable.Create(() =>
            {
                Unsubscribe(subscription);
            });
            subscription = new Subscription<T>(callBack, d);
            subscriptions.Add(subscription);
            return d;
        }
        else
        {
            return ((Subscription<T>)existingSubscription).Disposable;
        }
    }

    private bool Unsubscribe<T>(ISubscription<T> subscription) where T : IMessage
    {
        if (subscription is null)
            return false;

        var removed = false;
        var messageType = typeof(T);
        if (_observers.TryGetValue(messageType, out var collection))
        {
            collection.Remove(subscription);

            if (collection.Count == 0)
                _observers.Remove(messageType);
        }

        return removed;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        _isDisposed = true;

        foreach (var pair in _observers)
            pair.Value.Clear();
        _observers.Clear();
    }
}
