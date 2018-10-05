using System;

namespace DisposableEvents
{
    /// <summary>
    /// Optionally wraps another token, performing an additional action on dispose.
    /// </summary>
    public class TokenWithAction : IDisposable
    {
        private Action _onDispose;

        public TokenWithAction(Action onDispose)
        {
            _onDispose = onDispose;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Do NOT want this to be called when the token is just normally gc'd. rely on calling code for cleanup if that's the case
        /// </summary>
        public void Dispose()
        {
            _onDispose?.Invoke();
            _onDispose = null;
        }

        #endregion
    }
}