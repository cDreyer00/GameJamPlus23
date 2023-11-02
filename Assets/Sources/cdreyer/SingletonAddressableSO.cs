using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Sources.cdreyer
{
    public abstract class SingletonAddressableSO<T> : ScriptableObject where T : SingletonAddressableSO<T>
    {
        private static T _instance;
        protected static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(typeof(T).Name);
                    _instance = op.WaitForCompletion(); // Forces synchronous load

                    if (_instance == null)
                    {
                        GameLogger.GameLogger.Log($"{typeof(T).Name} instance not found", "red", "error");
                    }

                }

                return _instance;
            }
        }

        protected virtual T Standard { get; }

#if UNITY_EDITOR
        public static void FindObject()
        {
            UnityEditor.Selection.activeObject = Instance;
        }
#endif
    }
}