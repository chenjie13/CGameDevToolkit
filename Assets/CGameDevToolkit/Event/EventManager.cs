using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace CGameDevToolkit.Framework
{
    /// <summary>
    /// 简易的事件管理器
    /// 按监听事件类型来触发事件
    /// 
    /// 1 - 继承 IEventListener<Event> 
    /// 2 - OnEnable() { this.MMEventStartListening<Event>();}
    ///     OnDisable() { this.MMEventStopListening<Event>();}
    /// 3 - 实现 OnEvent
    /// </summary>
    public static class EventManager
    {
        private static Dictionary<Type, List<IEventListenerBase>> _subscribersDic;

        static EventManager()
        {
            _subscribersDic = new Dictionary<Type, List<IEventListenerBase>>();
        }

        public static void AddListener<TEvent>(IEventListener<TEvent> listener) where TEvent : struct
        {
            Type eventType = typeof(TEvent);
            if (!_subscribersDic.ContainsKey(eventType)) _subscribersDic[eventType] = new List<IEventListenerBase>();
            if (!SubscriptionExists(eventType, listener)) _subscribersDic[eventType].Add(listener);
        }

        public static void RemoveListener<TEvent>(IEventListener<TEvent> listener) where TEvent : struct
        {
            Type eventType = typeof(TEvent);

            if (!_subscribersDic.ContainsKey(eventType)) return;

            List<IEventListenerBase> subscriberList = _subscribersDic[eventType];

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)
                {
                    subscriberList.Remove(subscriberList[i]);
                    if (subscriberList.Count == 0) _subscribersDic.Remove(eventType);
                    return;
                }
            }
        }

        public static void TriggerEvent<TEvent>(TEvent newEvent) where TEvent : struct
        {
            List<IEventListenerBase> list;
            if (!_subscribersDic.TryGetValue(typeof(TEvent), out list))
            {
                UnityEngine.Debug.LogWarningFormat("Trigger a no listener event, type: {0}", typeof(TEvent));
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                ((IEventListener<TEvent>) list[i]).OnEvent(newEvent);
            }
        }

        private static bool SubscriptionExists(Type type, IEventListenerBase receiver)
        {
            List<IEventListenerBase> receivers;

            if (!_subscribersDic.TryGetValue(type, out receivers)) return false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i] == receiver) return true;
            }

            return false;
        }
    }
    

    // 管理注册的静态类
    public static class EventRegister
    {
        public static void EventStartListening<TEventType>(this IEventListener<TEventType> caller)
            where TEventType : struct
        {
            EventManager.AddListener(caller);
        }

        public static void EventStopListening<TEventType>(this IEventListener<TEventType> caller)
            where TEventType : struct
        {
            EventManager.RemoveListener(caller);
        }
    }
    

    public interface IEventListenerBase{};
    

    public interface IEventListener<in T> : IEventListenerBase
    {
        void OnEvent(T eventType);
    }
}