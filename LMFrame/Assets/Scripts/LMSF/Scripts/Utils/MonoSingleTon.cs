using UnityEngine;
namespace LMSF.Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return _instance;
                        }
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "Mono_" + typeof(T).ToString();
                            GameObject singletonRoot= GameObject.Find("MonoSingletonRoot");
                            if (singletonRoot == null)
                            {
                                singletonRoot = new GameObject("MonoSingletonRoot");
                            }
                            singleton.transform.SetParent(singletonRoot.transform, true);
                            DontDestroyOnLoad(singletonRoot);
                        }
                    }
                    return _instance;
                }
            }
        }
    }
}
