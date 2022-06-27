using UnityEngine;
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T mInstance = null;
        private static bool applicationIsQuitting = false;
        private static GameObject parent;

        public static T Instance
        {
            get
            {
                if (mInstance == null && !applicationIsQuitting)
                {
                    if (parent == null)
                    {
                        parent = GameObject.Find("Boot");
                        if (parent == null)
                        {
                            parent = new GameObject("Boot");
                            DontDestroyOnLoad(parent);
                        }
                    }
                    mInstance = parent.GetComponent<T>();
                    if (mInstance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        mInstance = go.AddComponent<T>();
                        if (parent)
                        {
                            go.transform.SetParent(parent.transform);
                        }
                    }
                }
                return mInstance;
            }
        }


        /// <summary>
        /// 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
        /// </summary>
        public void Startup()
        {
        }

        protected virtual void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this as T;
            }
            Destroy(this.gameObject);
            Init();
        }

        protected virtual void Init()
        {
        }

        public void DestroySelf()
        {
            Dispose();
            MonoSingleton<T>.mInstance = null;
            UnityEngine.Object.Destroy(gameObject);
        }

        public virtual void Dispose()
        {
        }

        public virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
