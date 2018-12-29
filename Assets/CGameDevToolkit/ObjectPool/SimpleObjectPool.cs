using System;
using System.Collections.Generic;
using CGameDevToolkit.Framework;
using UnityEngine;

namespace CGameDevToolkit.Framework
{
    /// <summary>
    /// 简易对象池
    /// </summary>
    public class SimpleObjectPool<T> : IPool<T>
    {
        public int MaxPoolCount = 20;

        public event Action<T> OnRespawn;
        public event Action<T> OnDespawn;

        protected Action<T> _destoryFunc;
        protected Func<T> _createFunc;
        protected Queue<T> _objectCaches;

        public SimpleObjectPool(Func<T> createFunc, Action<T> destoryFunc, int initCount = 0)
        {
            _objectCaches = new Queue<T>();
            _createFunc = createFunc;
            _destoryFunc = destoryFunc;
            
            for (int i = 0; i < initCount; i++)
            {
                Despawn(_createFunc());
            }
        }

        public virtual T Respawn()
        {
            var result = _objectCaches.Count > 0 ? _objectCaches.Dequeue() : _createFunc();
            if (OnRespawn != null) OnRespawn(result);
            return result;
        }

        public virtual void Despawn(T obj)
        {
            if (OnDespawn != null) OnDespawn(obj);
            if (_objectCaches.Count < MaxPoolCount)
            {
                _objectCaches.Enqueue(obj);
            }
            else if (_destoryFunc != null)
            {
                _destoryFunc(obj);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _objectCaches.Count; i++)
            {
                _destoryFunc(_objectCaches.Dequeue());
            }
        }
    }
}