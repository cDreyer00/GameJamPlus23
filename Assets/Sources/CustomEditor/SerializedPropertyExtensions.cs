#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions
{
    public static void SetDefaultRecursive(this SerializedProperty serializedProperty)
    {
        var property    = serializedProperty.Copy();
        var endProperty = serializedProperty.GetEndProperty();
        while (property.NextVisible(true) && !SerializedProperty.EqualContents(property, endProperty)) {
            switch (property.propertyType) {
            case SerializedPropertyType.Generic:
                property.SetDefaultRecursive();
                break;
            case SerializedPropertyType.ObjectReference:
                property.objectReferenceValue = default;
                break;
            case SerializedPropertyType.ArraySize:
                property.intValue = default;
                break;
            case SerializedPropertyType.Boolean:
                property.boolValue = default;
                break;
            case SerializedPropertyType.Bounds:
                property.boundsValue = default;
                break;
            case SerializedPropertyType.Character:
                property.intValue = default;
                break;
            case SerializedPropertyType.Color:
                property.colorValue = default;
                break;
            case SerializedPropertyType.Enum:
                property.enumValueIndex = default;
                break;
            case SerializedPropertyType.Float:
                property.floatValue = default;
                break;
            case SerializedPropertyType.Integer:
                property.intValue = default;
                break;
            case SerializedPropertyType.LayerMask:
                property.intValue = default;
                break;
            case SerializedPropertyType.Quaternion:
                property.quaternionValue = default;
                break;
            case SerializedPropertyType.Rect:
                property.rectValue = default;
                break;
            case SerializedPropertyType.String:
                property.stringValue = string.Empty;
                break;
            case SerializedPropertyType.Vector2:
                property.vector2Value = default;
                break;
            case SerializedPropertyType.Vector3:
                property.vector3Value = default;
                break;
            case SerializedPropertyType.Vector4:
                property.vector4Value = default;
                break;
            case SerializedPropertyType.ManagedReference:
                property.managedReferenceValue = default;
                break;
            default:
                Debug.LogError($"Unhandled property type {property.propertyType}");
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
#endif