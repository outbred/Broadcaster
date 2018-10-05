using System;
using System.Threading.Tasks;

namespace DisposableEvents
{
    public interface IBroadcaster : IDisposable
    {
        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed, else an IDisposable token</returns>
        IDisposable Listen<TMessageType>(TMessageType message, Func<Task> onBroadcast) where TMessageType : System.Enum;

        /// <summary>
        /// Broadcast Enum based messages. Awaits each subscriber in order that they subscribed
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task Broadcast<TMessageType>(TMessageType message) where TMessageType : System.Enum;
    }
}