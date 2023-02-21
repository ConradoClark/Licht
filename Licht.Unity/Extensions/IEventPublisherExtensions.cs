using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class IEventPublisherExtensions
    {
        public static IEventPublisher<TEvent, TArgs> RegisterEventPublisher<TEvent, TArgs>(this IEventPublisher<TEvent,TArgs> obj, MonoBehaviour source)
        {
            return source.RegisterAsEventPublisher<TEvent, TArgs>();
        }
    }
}