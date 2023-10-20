using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cdreyer
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new($"{typeof(T).Name}_Singleton");
                    _instance = go.AddComponent<T>();
                }

                return _instance;
            }
        }

        [SerializeField] bool dontDestroyOnLoad = true;

        // [SerializeField] bool InstantiateIfNull { get => false; }

        protected virtual void Awake()
        {
            if (_instance == this) return;
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;

            if (dontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(this);
            }
        }

        public virtual void Dispose() { Destroy(gameObject); }
    }
}