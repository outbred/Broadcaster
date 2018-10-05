using System;

namespace DisposableEvents
{
    /// <summary>
    /// When disposed, performs some action
    /// </summary>
    internal class ListenerToken : IDisposable
    {
        private Action _onDispose;

        public ListenerToken(Action onDispose)
        {
            _onDispose = onDispose;
        }

        #region IDisposable

        public void Dispose()
        {
            if (_onDispose != null)
            {
                _onDispose();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}