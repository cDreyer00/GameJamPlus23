namespace Sources
{
    public static class GameObjectExt
    {
        /// <summary>
        /// Returns a 'true null' if UnityObject is considered null by Unity. 
        /// </summary>
        public static T OrNull<T>(this T obj) where T : UnityEngine.Object => obj ? obj : null;
    }
}