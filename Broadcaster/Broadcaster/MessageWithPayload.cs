using System;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcaster
{
    public abstract class BaseMessage
    {
        public BroadcasterWithPayload Broadcaster { get; protected internal set; }
    }

    /// <summary>
    /// When using a payload with your message, this serves as the base class for the message. Think PubSub<TType>
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public abstract class MessageWithPayload<TPayload> : BaseMessage
    {
        /// <summary>
        /// Listen for this message with a strongly typed payload
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="onBroadcast"></param>
        /// <returns>A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        public IDisposable Listen(Func<TPayload, Task> onBroadcast)
        {
            var tuple = new Tuple<Type, Func<object, Task>>(this.GetType(), async arg => await onBroadcast((TPayload)arg));
            if (Broadcaster._subscribers.Contains(tuple))
                return null;

            Broadcaster._subscribers.Add(tuple);

            var token = new ListenerToken(() => Broadcaster._subscribers.Remove(tuple));
            return token;
        }

        /// <summary>
        /// Broadcast this messages with a strongly typed payload. Each subscriber is awaited in order.
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task Broadcast(TPayload payload)
        {
            var messageType = this.GetType();
            foreach (var match in Broadcaster._subscribers.Where(p => p.Item1 == messageType))
            {
                try
                {
                    await match.Item2(payload);
                }
                catch (Exception ex)
                {
                    // todo: log it
                }
            }
        }
    }
}