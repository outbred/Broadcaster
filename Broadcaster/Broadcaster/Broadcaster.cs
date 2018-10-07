using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcaster
{
    /// <summary>
    /// An enum based broadcaster/subscriber class with no payloads.  All events/messages are enums, of your own type (ideally specific to
    /// the class that has this instance).
    ///
    /// Why enums only?  Keeps the incarnations of this type endless, but provides compile-time knowledge of the message type.
    ///
    /// No need for a base class since there's no payload.
    ///
    /// All listeners to the event will be awaited in order as they subscribe.
    /// </summary>
    public class Broadcaster<TMessageType> where TMessageType : System.Enum
    {
        protected internal readonly List<Tuple<System.Enum, Func<Task>>> _subscribers = new List<Tuple<System.Enum, Func<Task>>>();

        /// <summary>
        /// Broadcast Enum based messages. Awaits each subscriber in order that they subscribed
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task Broadcast(TMessageType message)
        {
            var messageString = message.ToString();
            foreach (var match in _subscribers.Where(p => p.Item1.ToString() == messageString))
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
