using System;
using System.Threading.Tasks;

namespace Explorer.Interfaces.MessageBus;

public interface ISubscription<T> where T : IMessage
{
    Func<T, Task> Handler { get; }
}
