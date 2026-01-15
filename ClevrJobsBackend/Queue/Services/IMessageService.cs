using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Services
{
    public interface IMessageService
    {
        Task PublishAsync<T>(T message);
        Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken);
    }
}
