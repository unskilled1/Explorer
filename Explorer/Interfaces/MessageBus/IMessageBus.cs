using System;
using System.Threading.Tasks;

namespace Explorer.Interfaces.MessageBus;

public interface IMessageBus : IDisposable
{
    Task SendAsync<T>(T message) where T : IMessage;
    IDisposable Subscribe<T>(Func<T, Task> callBack) where T : IMessage;
}
