using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
#endif

[Serializable]
public struct ClampedPrimitive<TValue> where TValue : unmanaged, IComparable<TValue>, IEquatable<TValue>
{
    [SerializeField] TValue value;
    public           TValue min;
    public           TValue max;
    public ClampedPrimitive(TValue value, TValue min, TValue max)
    {
        this.value = Clamp(value, min, max);
        this.min = min;
        this.max = max;
    }
    public void Clamp() => value = Clamp(value, min, max);
    public TValue Value
    {
        get => value = Clamp(value, min, max);
        set => this.value = Clamp(value, min, max);
    }
    public static TValue Clamp(TValue value, TValue min, TValue max)
    {
        if (value.CompareTo(min) <= 0) {
            return min;
        }
        else if (value.CompareTo(max) >= 0) {
            return max;
        }
        return value;
    }

    public static implicit operator TValue(ClampedPrimitive<TValue> primitive) => primitive.Value;
}


#region Editor

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ClampedPrimitive<float>))]
public class ClampedPrimitiveFloatDrawer : PropertyDrawer
{
    const    float      SubLabelSpacing = 4;
    const    float      BottomSpacing   = 2;
    readonly GUIContent _minLabel       = new("Min");
    readonly GUIContent _maxLabel       = new("Max");
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        pos.height -= BottomSpacing;

        var value = prop.FindPropertyRelative("value");
        var min   = prop.FindPropertyRelative("min");
        var max   = prop.FindPropertyRelative("max");

        if (value == null || min == null || max == null) {
            EditorGUI.LabelField(pos, label, EditorGUIUtility.TrTempContent("Type is not serializable"));
            return;
        }

        var fieldType   = fieldInfo.FieldType;
        var clampMethod = fieldType.GetMethod("Clamp", BindingFlags.Static | BindingFlags.Public);

        var args = new[] { value.boxedValue, min.boxedValue, max.boxedValue, };
        value.boxedValue = clampMethod!.Invoke(null, args);

        EditorGUI.indentLevel = 0;
        var indent = EditorGUI.indentLevel;

        using (EditorGUILayouts.Horizontal()) {
            value.floatValue = EditorGUILayout.Slider(label, value.floatValue, min.floatValue, max.floatValue);

            var labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(_minLabel).x + SubLabelSpacing;
            var widthOption = GUILayout.Width(EditorGUIUtility.currentViewWidth / 10);

            min.floatValue = EditorGUILayout.FloatField(_minLabel, min.floatValue, widthOption);
            max.floatValue = EditorGUILayout.FloatField(_maxLabel, max.floatValue, widthOption);

            EditorGUIUtility.labelWidth = labelWidth;
        }

        EditorGUI.indentLevel = indent;
    }
}
[CustomPropertyDrawer(typeof(ClampedPrimitive<double>))]
public class ClampedPrimitiveDoubleDrawer : PropertyDrawer
{
    const    float      SubLabelSpacing = 4;
    const    float      BottomSpacing   = 2;
    readonly GUIContent _minLabel       = new("Min");
    readonly GUIContent _maxLabel       = new("Max");
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        pos.height -= BottomSpacing;

        var value = prop.FindPropertyRelative("value");
        var min   = prop.FindPropertyRelative("min");
        var max   = prop.FindPropertyRelative("max");

        if (value == null || min == null || max == null) {
            EditorGUI.LabelField(pos, label, EditorGUIUtility.TrTempContent("Type is not serializable"));
            return;
        }

        var fieldType   = fieldInfo.FieldType;
        var clampMethod = fieldType.GetMethod("Clamp", BindingFlags.Static | BindingFlags.Public);

        var args = new[] { value.boxedValue, min.boxedValue, max.boxedValue, };
        value.boxedValue = clampMethod!.Invoke(null, args);

        EditorGUI.indentLevel = 0;
        var indent = EditorGUI.indentLevel;

        using (EditorGUILayouts.Horizontal()) {
            value.doubleValue = EditorGUILayout.Slider(label, (float)value.doubleValue, (float)min.doubleValue, (float)max.doubleValue);

            var labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(_minLabel).x + SubLabelSpacing;
            var widthOption = GUILayout.Width(EditorGUIUtility.currentViewWidth / 10);

            min.doubleValue = EditorGUILayout.FloatField(_minLabel, (float)min.doubleValue, widthOption);
            max.doubleValue = EditorGUILayout.FloatField(_maxLabel, (float)max.doubleValue, widthOption);

            EditorGUIUtility.labelWidth = labelWidth;
        }

        EditorGUI.indentLevel = indent;
    }
}
#endif

#endregion