#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class EditorGUILayouts
{
    public static EditorGUILayoutVertical Vertical(params GUILayoutOption[] options) => new(options);
    public static EditorGUILayoutVertical Vertical(GUIStyle style, params GUILayoutOption[] options) => new(style, options);
    public static EditorGUILayoutHorizontal Horizontal(params GUILayoutOption[] options) => new(options);
    public static EditorGUILayoutHorizontal Horizontal(GUIStyle style, params GUILayoutOption[] options) => new(style, options);
    public static EditorGUILayoutScrollView ScrollView(ref Vector2 s, params GUILayoutOption[] options) => new(ref s, options);
    public static EditorGUILayoutFadeGroup FadeGroup(float value) => new(value);
    public static EditorGUILayoutToggleGroup ToggleGroup(string label, bool toggle) => new(label, toggle);
    public readonly struct EditorGUILayoutVertical : IDisposable
    {
        public EditorGUILayoutVertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }
        public EditorGUILayoutVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }
        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }
    public readonly struct EditorGUILayoutHorizontal : IDisposable
    {
        public EditorGUILayoutHorizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }
        public EditorGUILayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }
        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
    public readonly struct EditorGUILayoutScrollView : IDisposable
    {
        public EditorGUILayoutScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
        }
        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }
    public readonly struct EditorGUILayoutFadeGroup : IDisposable
    {
        public EditorGUILayoutFadeGroup(float value)
        {
            EditorGUILayout.BeginFadeGroup(value);
        }
        public void Dispose()
        {
            EditorGUILayout.EndFadeGroup();
        }
    }
    public readonly struct EditorGUILayoutToggleGroup : IDisposable
    {
        public EditorGUILayoutToggleGroup(string label, bool toggle)
        {
            EditorGUILayout.BeginToggleGroup(label, toggle);
        }
        public void Dispose()
        {
            EditorGUILayout.EndToggleGroup();
        }
    }
}

#endif