#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializeReadOnlyAttribute))]
public class DisplayReadOnlyAttributeDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        GUI.enabled = false;
    }
}
#endif
public class SerializeReadOnlyAttribute : PropertyAttribute {}