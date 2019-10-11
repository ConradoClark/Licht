using System;
using Licht.Interfaces.Memory;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Interfaces.Events;

namespace Licht.Impl.Memory
{
    [PublicAPI]
    public class Caterpillar<T> : ICaterpillar<T>,
        IEventObservable<CaterpillarEvents, T>
    {
        private readonly List<T> _stack = new List<T>();
        public int TailSize { get; set; }
        public T Current { get => _stack.LastOrDefault(); set => _add(value); }

        public event Action<T> BeforeAdd;
        public event Action<T> AfterAdd;
        public event Action<T> OnTrailRemove;

        public Caterpillar(bool enableEvents = false)
        {
            _add = enableEvents ? (Action<T>) AddInternalWithEvents : AddInternal;
        }

        public int Length => _stack.Count;

        public T GetTrail(int index)
        {
            return _stack.Count > index-1 ? _stack[_stack.Count - index] : default(T);
        }

        public bool HasTrail(int index)
        {
            return _stack.Count > index - 1;
        }

        public void Rewind(int steps)
        {
            if (_stack.Count < steps || TailSize < steps) return;
            _stack.RemoveRange(0, steps);
        }

        private readonly Action<T> _add;
        private void AddInternal(T value)
        {
            _stack.Add(value);
            if (_stack.Count > TailSize && TailSize > 0)
            {
                _stack.RemoveAt(0);
            }
        }

        private void AddInternalWithEvents(T value)
        {
            BeforeAdd?.Invoke(value);
            _stack.Add(value);
            AfterAdd?.Invoke(value);

            if (_stack.Count <= TailSize || TailSize <= 0) return;

            var obj = _stack[0];
            _stack.RemoveAt(0);
            OnTrailRemove?.Invoke(obj);
        }

        public void ObserveEvent(CaterpillarEvents eventName, Action<T> onEvent)
        {
            switch (eventName)
            {
                case CaterpillarEvents.BeforeAdd: BeforeAdd += onEvent;
                    return;
                case CaterpillarEvents.AfterAdd: AfterAdd += onEvent;
                    return;
                case CaterpillarEvents.OnTrailRemove: OnTrailRemove += onEvent;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null);
            }
        }

        public void StopObservingEvent(CaterpillarEvents eventName, Action<T> onEvent)
        {
            switch (eventName)
            {
                case CaterpillarEvents.BeforeAdd:
                    BeforeAdd -= onEvent;
                    return;
                case CaterpillarEvents.AfterAdd:
                    AfterAdd -= onEvent;
                    return;
                case CaterpillarEvents.OnTrailRemove:
                    OnTrailRemove -= onEvent;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null);
            }
        }

        public void StopObservingAllEvents()
        {
            BeforeAdd = null;
            AfterAdd = null;
            OnTrailRemove = null;
        }
    }

}