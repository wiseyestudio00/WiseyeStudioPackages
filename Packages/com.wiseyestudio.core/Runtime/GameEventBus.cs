using System;
using System.Collections.Generic;
using WiseyeStudio.Singleton;

namespace WiseyeStudio
{
    public class GameEventBus : DontDestroyOnLoadSingleton<GameEventBus>
    {
        private readonly Dictionary<Type, Delegate> _eventHandlers = new();

        public void Subscribe<T>(Action<T> gameEvent) where T : class
        {
            var eventType = typeof(T);
            if (_eventHandlers.TryAdd(eventType, gameEvent))
            {
                return;
            }

            _eventHandlers[eventType] = Delegate.Combine(_eventHandlers[eventType], gameEvent);
        }

        public void Unsubscribe<T>(Action<T> gameEvent) where T : class
        {
            var eventType = typeof(T);
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                _eventHandlers[eventType] = Delegate.Remove(handlers, gameEvent);
            }
        }

        public void Publish<T>(T eventData) where T : class
        {
            if (_eventHandlers.TryGetValue(typeof(T), out var handlers) == false)
            {
                return;
            }

            (handlers as Action<T>)?.Invoke(eventData);
        }
    }
}
