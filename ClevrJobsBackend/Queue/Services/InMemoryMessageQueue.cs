using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Queue.Services
{
    public class InMemoryMessageQueue : IMessageQueue
    {
        private readonly ConcurrentDictionary<Type, object> _channels = new();

        public async Task PublishAsync<T>(T message)
        {
            var channel = GetChannel<T>();
            await channel.Writer.WriteAsync(message);
        }

        public async Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken)
        {
            var channel = GetChannel<T>();
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await handler(message);
            }
        }

        private Channel<T> GetChannel<T>()
        {
            return (Channel<T>)_channels.GetOrAdd(typeof(T),
                _ => Channel.CreateUnbounded<T>());
        }
    }
}
