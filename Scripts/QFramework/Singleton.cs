// #define QFRAMEWORK// 在没有QFramework时，提供基础的简易单例模式框架

#if !QFRAMEWORK
using UnityEngine;

namespace UI
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static T mInstance;
        static object mLock = new object();
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    mInstance ??= new T();
                }

                return mInstance;
            }
        }
        public virtual void Dispose()
        {
            mInstance = null;
        }
        public virtual void OnSingletonInit()
        {
        }
    }
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T mInstance;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    //不同于QF，不会自动尝试生成对象
                    mInstance = FindObjectOfType(typeof(T)) as T;
                    if (mInstance != null)
                    {
                        mInstance.OnSingletonInit();
                        return mInstance;
                    }
                }
                return mInstance;
            }
        }

        public virtual void OnSingletonInit()
        {
        }
        public virtual void Dispose()
        {
            mInstance = null;
        }

        protected virtual void OnApplicationQuit()
        {
            if (mInstance == null) return;
            Destroy(mInstance.gameObject);
            mInstance = null;
        }
        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
#endif