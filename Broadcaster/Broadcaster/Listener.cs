using System;
using System.Threading.Tasks;

namespace Broadcaster
{
    public class Listener<TMessageType> where TMessageType : System.Enum
    {
        private readonly WeakReference<Broadcaster<TMessageType>> _broadcaster;

        public Listener(Broadcaster<TMessageType> broadcaster)
        {
            _broadcaster = new WeakReference<Broadcaster<TMessageType>>(broadcaster);
        }

        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed, else an IDisposable token. Caller is responsible for lifetime of token!</returns>
        public IDisposable Listen(TMessageType message, Func<Task> onBroadcast)
        {
            if (!_broadcaster.TryGetTarget(out var broadcaster))
                return null;

            var tuple = new Tuple<System.Enum, Func<Task>>(message, onBroadcast);
            if (broadcaster._subscribers.Contains(tuple))
                return null;

            broadcaster._subscribers.Add(tuple);

            var token = new ListenerToken(() => broadcaster._subscribers.Remove(tuple));
            return token;
        }
    }
}