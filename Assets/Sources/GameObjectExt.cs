using UnityEngine;

namespace Sources
{
    public static class GameObjectExt
    {
        /// <summary>
        /// Returns a 'true null' if UnityObject is considered null by Unity. 
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        public static bool TryGetComponent<T>(this Object obj, out T component) where T : Object
        {
            if (obj is GameObject go) {
                component = go.GetComponent<T>();
                return component;
            }
            if (obj is Component co) {
                component = co.GetComponent<T>();
                return component;
            }
            component = null;
            return false;
        }
    }
}