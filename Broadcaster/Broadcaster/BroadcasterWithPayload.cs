using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisposableEvents
{
    /// <summary>
    /// For use if you have messages to broadcast that also have payloads or data associated with them.  Can be used with enums, as well,
    /// but you lose strong type support b/c of Microsoft's C# implementation limitations.
    ///
    /// With 7.0.3 C# language support, at least that only happens with enum-based messages.  Can still retain strong type safety with object
    /// based messages that inherit from <typeparam name="MessageWithPayload"></typeparam>
    ///
    /// All listeners to the message will be awaited in order as they subscribe.
    /// </summary>
    public class BroadcasterWithPayload : IDisposable
    {
        private readonly List<Tuple<object, Func<object, Task>>> _subscribers = new List<Tuple<object, Func<object, Task>>>();

        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed!  A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        public IDisposable Listen<TMessageType>(TMessageType message, Func<object, Task> onBroadcast) where TMessageType : System.Enum
        {
            var tuple = new Tuple<object, Func<object, Task>>(message, onBroadcast);
            if (_subscribers.Contains(tuple))
                return null;

            _subscribers.Add(tuple);

            var token = new ListenerToken(() => _subscribers.Remove(tuple));
            return token;
        }

        /// <summary>
        /// Listen for object-based messages with strongly typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        public IDisposable Listen<TMessageType, TPayload>(TMessageType message, Func<TPayload, Task> onBroadcast) where TMessageType : MessageWithPayload<TPayload>
        {
            var tuple = new Tuple<object, Func<object, Task>>(message, async arg => await onBroadcast((TPayload)arg));
            if (_subscribers.Contains(tuple))
                return null;

            _subscribers.Add(tuple);

            var token = new ListenerToken(() => _subscribers.Remove(tuple));
            return token;
        }

        /// <summary>
        /// Broadcast Enum based messages with a payload. Each subscriber is awaited in order.
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task Broadcast<TMessageType>(TMessageType message, object payload) where TMessageType : System.Enum
        {
            foreach (var match in _subscribers.Where(p => p.Item1 == (object)message))
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

        /// <summary>
        /// Broadcast object messages with a strongly typed payload. Each subscriber is awaited in order.
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task Broadcast<TMessageType, TPayload>(TMessageType message, TPayload payload) where TMessageType : MessageWithPayload<TPayload>
        {
            var messageType = message.GetType();
            foreach (var match in _subscribers.Where(p => p.Item1.GetType() == messageType))
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