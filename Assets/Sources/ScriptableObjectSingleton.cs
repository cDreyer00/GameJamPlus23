using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
{ 
    static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var instances = Resources.FindObjectsOfTypeAll<T>();
                if (instances.Length == 0)
                {
                    Debug.LogError($"No instance of {typeof(T).Name} found in resources");
                    return null;
                }
                if (instances.Length > 1)
                {
                    Debug.LogError($"Multiple instances of {typeof(T).Name} found in resources");
                    return null;
                }
                _instance = instances[0];
                _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }
            return _instance;
        }
    }
}
