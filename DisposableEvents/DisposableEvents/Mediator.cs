using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisposableEvents
{
    /// <summary>
    /// Version of EventManager that also takes a parameter and has awaitable handlers
    /// </summary>
    /// <typeparam name="TEventType"></typeparam>
    public class Mediator<TEventType> : IDisposable
    {
        private object _locker = new object();
        private readonly List<Tuple<TEventType, Func<object, Task>>> _actions = new List<Tuple<TEventType, Func<object, Task>>>();
        private TEventType _disposeEvent;
        private bool _disposed = false;

        public Mediator(TEventType disposeEvent)
        {
            _disposeEvent = disposeEvent;
        }

        public IDisposable Register(TEventType evt, Func<object, Task> action)
        {
            if (_disposed)
            {
                action(null);
                return new TokenWithAction(() => { });
            }

            var tuple = new Tuple<TEventType, Func<object, Task>>(evt, action);
            lock (_locker)
            {
                if (!_actions.Contains(tuple))
                {
                    _actions.Add(tuple);
                    return new TokenWithAction(() =>
                    {
                        lock(_locker)
                            _actions.Remove(tuple);
                    });
                }
            }

            return null;
        }

        public async Task Fire(TEventType evt, object payload)
        {
            if (_actions == null)
                return;

            foreach (var action in _actions.ToList().Where(a => a.Item1?.Equals(evt) ?? false))
            {
                if (action.Item2 != null)
                    await action.Item2(payload);
            }
        }

        #region IDisposable

        /// <summary>
        /// This will fire the Dispose event passed into the ctor as well
        /// </summary>
        public async void Dispose()
        {
            await Fire(_disposeEvent, this);
            _actions.Clear();
            _disposed = true;
        }

        #endregion
    }
}
