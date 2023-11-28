using UnityEngine;

namespace Sources.cdreyer
{
    public abstract class SingletonSO<T> : ScriptableObject where T : SingletonSO<T>
    {
        private static T _instance;
        protected static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GlobalInstancesBehaviour.GlobalInstances.GetInstance<T>(CreateInstance<T>().IdStr); // Forces synchronous load

                    if (_instance == null)
                    {
                        GameLogger.Log($"{typeof(T).Name} instance not found", "red", "error");
                    }

                }

                return _instance;
            }
        }

        protected virtual T Standard { get; }

        public abstract string IdStr { get; }

#if UNITY_EDITOR
        public static void FindObject()
        {
            UnityEditor.Selection.activeObject = Instance;
        }
#endif
    }
}