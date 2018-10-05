namespace DisposableEvents
{
    /// <summary>
    /// When using a payload with your message, this serves as the base class for the message. Think PubSub<TType>
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public abstract class MessageWithPayload<TPayload>
    {
    }
}