using System;
using System.Threading.Tasks;

namespace DisposableEvents
{
    public interface IBroadcasterWithPayload : IDisposable
    {
        /// <summary>
        /// Listen for enum-based messages with loosely typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>Null if already subscribed!  A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        IDisposable Listen<TMessageType>(TMessageType message, Func<object, Task> onBroadcast) where TMessageType : System.Enum;

        /// <summary>
        /// Listen for object-based messages with strongly typed payloads
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="message"></param>
        /// <param name="onBroadcast"></param>
        /// <returns>A disposable token. After it is disposed, the message handler is removed. Caller must maintain a reference to the token!</returns>
        IDisposable Listen<TMessageType, TPayload>(TMessageType message, Func<TPayload, Task> onBroadcast) where TMessageType : MessageWithPayload<TPayload>;

        /// <summary>
        /// Broadcast Enum based messages with a payload. Each subscriber is awaited in order.
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task Broadcast<TMessageType>(TMessageType message, object payload) where TMessageType : System.Enum;

        /// <summary>
        /// Broadcast object messages with a strongly typed payload. Each subscriber is awaited in order.
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task Broadcast<TMessageType, TPayload>(TMessageType message, TPayload payload) where TMessageType : MessageWithPayload<TPayload>;
    }
}