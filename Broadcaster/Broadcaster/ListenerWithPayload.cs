using System;
using System.Threading.Tasks;

namespace Broadcaster
{
    public class ListenerWithPayload<TMessageType> where TMessageType : System.Enum
    {
        private readonly WeakReference<BroadcasterWithPayload<TMessageType>> _broadcaster;

        public ListenerWithPayload(BroadcasterWithPayload<TMessageType> broadcaster)
        {
            _broadcaster = new WeakReference<BroadcasterWithPayload<TMessageType>>(broadcaster);
        }

        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed!  A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        public IDisposable Listen(TMessageType message, Func<object, Task> onBroadcast)
        {
            if (!_broadcaster.TryGetTarget(out var broadcaster))
                return null;

            var tuple = new Tuple<Enum, Func<object, Task>>(message, onBroadcast);
            if (broadcaster._subscribers.Contains(tuple))
                return null;

            broadcaster._subscribers.Add(tuple);

            var token = new ListenerToken(() => broadcaster._subscribers.Remove(tuple));
            return token;
        }
    }
}