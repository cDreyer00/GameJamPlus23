using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GlobalInstancesBehaviour : MonoBehaviour
{
    [SerializeField] SerializableKVP<string, Object>[] keyObjects;

    static GlobalInstances<Object> _globalInstances;
    public static GlobalInstances<Object> GlobalInstances
    {
        get
        {
            if (_globalInstances == null)
            {
                // try get from scene
                if (!Helpers.TryFindObjectOfType<GlobalInstancesBehaviour>(out var ilb))
                    // try get from Resources
                    ilb = Resources.Load<GlobalInstancesBehaviour>("Global_Instances");

                if (ilb == null)
                {
                    Debug.LogError("Global_Instances could not be found");
                    return null;
                }

                _globalInstances = new();
                foreach (var ko in ilb.keyObjects)
                    _globalInstances.AddInstance(ko.key, ko.value);

                Debug.Log("Global instances applied");
            }

            return _globalInstances;
        }
    }

    void OnValidate()
    {
        for (int i = 0; i < keyObjects.Length; i++)
        {
            var obj = keyObjects[i];
            obj.key = obj.value.name;
            keyObjects[i] = obj;
        }
    }    
}
