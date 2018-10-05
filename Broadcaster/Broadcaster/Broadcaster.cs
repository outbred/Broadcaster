using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisposableEvents
{
    /// <summary>
    /// An enum based broadcaster/subscriber class with no payloads.  All events/messages are enums, of your own type (ideally specific to
    /// the class that has this instance).
    ///
    /// All listeners to the event will be awaited in order as they subscribe.
    /// </summary>
    public class Broadcaster : IBroadcaster
    {
        private readonly List<Tuple<object, Func<Task>>> _subscribers = new List<Tuple<object, Func<Task>>>();

        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed, else an IDisposable token. Caller is responsible for lifetime of token!</returns>
        public IDisposable Listen<TMessageType>(TMessageType message, Func<Task> onBroadcast) where TMessageType : System.Enum
        {
            var tuple = new Tuple<object, Func<Task>>(message, onBroadcast);
            if (_subscribers.Contains(tuple))
                return null;

            _subscribers.Add(tuple);

            var token = new ListenerToken(() => _subscribers.Remove(tuple));
            return token;
        }

        /// <summary>
        /// Broadcast Enum based messages. Awaits each subscriber in order that they subscribed
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task Broadcast<TMessageType>(TMessageType message) where TMessageType : System.Enum
        {
            foreach (var match in _subscribers.Where(p => p.Item1 == (object)message))
            {
                try
                {
                    await match.Item2();
                }
                catch (Exception ex)
                {
                    // todo: log it
                }
            }
        }

        #region IDisposable

        public void Dispose()
        {
            if (_subscribers.Any())
            {
                _subscribers.Clear();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
