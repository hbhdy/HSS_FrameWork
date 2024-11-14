using UnityEngine;

namespace HSS
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj;
                    obj = GameObject.Find(typeof(T).Name);
                    if (obj == null)
                    {
                        obj = new GameObject(typeof(T).Name);
                        instance = obj.AddComponent<T>();
                    }
                    else
                    {
                        instance = obj.GetComponent<T>();
                    }
                }

                return instance;
            }
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Destroy()
        {
            Destroy(instance);
            instance = null;
        }

        public static T GetInstance() { return instance; }
    }

    public class Singleton<T> where T : class, new()
    {
        private static readonly T _instance = new T();
        public static T Instance { get { return _instance; } }

    }
}