using UnityEngine;

namespace KvinterGames
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindObjectOfType<T>();
                if (instance != null) return instance;
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
                OnInitialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                OnDeinitialize();
            }
        }

        protected virtual void OnInitialize()
        {
        }
        
        protected virtual void OnDeinitialize()
        {
        }
    }
}