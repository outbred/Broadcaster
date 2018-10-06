using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcaster
{
    /// <summary>
    /// For use if you have messages to broadcast that also have payloads or data associated with them.  All messages are Enums,
    /// so you lose strong type support in the payload b/c of Microsoft's C# implementation limitations.
    ///
    /// All listeners to the message will be awaited in order as they subscribe.
    /// </summary>
    public class BroadcasterWithPayload<TMessageType> where TMessageType : System.Enum
    {
        private readonly List<Tuple<object, Func<object, Task>>> _subscribers = new List<Tuple<object, Func<object, Task>>>();

        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed!  A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        public IDisposable Listen(TMessageType message, Func<object, Task> onBroadcast)
        {
            var tuple = new Tuple<object, Func<object, Task>>(message, onBroadcast);
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
        public async Task Broadcast(TMessageType message, object payload)
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

    /// <summary>
    /// For use if you have messages to broadcast that also have payloads or data associated with them AND your messages are not Enum values. Follows the Event Aggregator pattern pretty closely
    /// b/c there's no other way to do it with strong-type safety in the current implementation of C# w/o an annoying level of verbosity with each call
    /// (even more than it is now)
    ///
    /// All listeners to the message will be awaited in order as they subscribe.
    /// </summary>
    public class BroadcasterWithPayload
    {
        internal readonly List<Tuple<Type, Func<object, Task>>> _subscribers = new List<Tuple<Type, Func<object, Task>>>();

        public TMessageType Get<TMessageType>() where TMessageType : BaseMessage, new()
        {
            var evt = new TMessageType {Broadcaster = this};
            return evt;
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