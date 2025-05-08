// #define QFRAMEWORK// 在没有QFramework时，提供基础的简易单例模式框架

#if !QFRAMEWORK
using System;

namespace UI
{
    public class EasyEvent
    {
        private Action mOnEvent = () => { };

        public void Register(Action onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegister(Action onEvent) => mOnEvent -= onEvent;
        public void UnRegisterAll() => mOnEvent = null;
        public void Trigger() => mOnEvent?.Invoke();
    }

    public class EasyEvent<T>
    {
        private Action<T> mOnEvent = e => { };

        public void Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;
        public void UnRegisterAll() => mOnEvent = null;
        public void Trigger(T t) => mOnEvent?.Invoke(t);

        public void Register(Action onEvent)
        {
            Register(Action);
            void Action(T _) => onEvent();
        }
    }
}
#endif