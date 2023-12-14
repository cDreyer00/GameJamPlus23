using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ScriptableObjectEvent", menuName = "ScriptableObjectEvent")]
public class ScriptableObjectEvent : ScriptableObject
{
    public List<EventListener> listeners = new();
    public void Invoke(Object sender, object args)
    {
        for (int i = listeners.Count - 1; i >= 0; i--) {
            listeners[i].OnInvoke(sender, args);
        }
    }
    public void AddListener(EventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }
    public bool RemoveListener(EventListener listener) => listeners.Remove(listener);
    public void RemoveAllListeners() => listeners.Clear();
    public void AddListener(Action<Object, object> listener)
    {
        var eventListener = EventListener.CreateBinding(this, listener);
        if (!listeners.Contains(eventListener))
            listeners.Add(eventListener);
    }
    public void AddListener(Action listener)
    {
        var eventListener = EventListener.CreateBinding(this, listener);
        if (!listeners.Contains(eventListener))
            listeners.Add(eventListener);
    }
    public void RemoveListener(Action<Object, object> listener)
    {
        var eventListener = EventListener.CreateBinding(this, listener);
        listeners.Remove(eventListener);
    }
    public void RemoveListener(Action listener)
    {
        var eventListener = EventListener.CreateBinding(this, listener);
        listeners.Remove(eventListener);
    }

    public static ScriptableObjectEvent operator +(ScriptableObjectEvent scriptableObjectEvent, EventListener listener)
    {
        scriptableObjectEvent.AddListener(listener);
        return scriptableObjectEvent;
    }
    public static ScriptableObjectEvent operator -(ScriptableObjectEvent scriptableObjectEvent, EventListener listener)
    {
        scriptableObjectEvent.RemoveListener(listener);
        return scriptableObjectEvent;
    }
    public static ScriptableObjectEvent operator +(ScriptableObjectEvent scriptableObjectEvent, Action<Object, object> listener)
    {
        scriptableObjectEvent.AddListener(listener);
        return scriptableObjectEvent;
    }
    public static ScriptableObjectEvent operator -(ScriptableObjectEvent scriptableObjectEvent, Action<Object, object> listener)
    {
        scriptableObjectEvent.RemoveListener(listener);
        return scriptableObjectEvent;
    }
    public static ScriptableObjectEvent operator +(ScriptableObjectEvent scriptableObjectEvent, Action listener)
    {
        scriptableObjectEvent.AddListener(listener);
        return scriptableObjectEvent;
    }
    public static ScriptableObjectEvent operator -(ScriptableObjectEvent scriptableObjectEvent, Action listener)
    {
        scriptableObjectEvent.RemoveListener(listener);
        return scriptableObjectEvent;
    }

    #region Editor

    #if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableObjectEvent))]
    public class ScriptableObjectEventEditor : Editor
    {
        enum EventArgumentType
        {
            None, Object, Int,
            Float, String, Bool,
            Vector2, Vector3, Vector4,
            Color, Rect, Bounds
        }
        EventArgumentType _eventArgumentType;
        object            _eventArgument;
        Object            _sender;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var scriptableObjectEvent = (ScriptableObjectEvent)target;
            _sender = EditorGUILayout.ObjectField("Sender", _sender, typeof(Object), true);
            _eventArgumentType = (EventArgumentType)EditorGUILayout.EnumPopup("Argument Type", _eventArgumentType);
            _eventArgument = _eventArgumentType switch
            {
                EventArgumentType.Object  => EditorGUILayout.ObjectField("Argument", (Object)_eventArgument, typeof(Object), true),
                EventArgumentType.Int     => EditorGUILayout.IntField("Argument", _eventArgument as int? ?? 0),
                EventArgumentType.Float   => EditorGUILayout.FloatField("Argument", _eventArgument as float? ?? 0f),
                EventArgumentType.String  => EditorGUILayout.TextField("Argument", _eventArgument as string ?? string.Empty),
                EventArgumentType.Bool    => EditorGUILayout.Toggle("Argument", _eventArgument as bool? ?? false),
                EventArgumentType.Vector2 => EditorGUILayout.Vector2Field("Argument", _eventArgument as Vector2? ?? Vector2.zero),
                EventArgumentType.Vector3 => EditorGUILayout.Vector3Field("Argument", _eventArgument as Vector3? ?? Vector3.zero),
                EventArgumentType.Vector4 => EditorGUILayout.Vector4Field("Argument", _eventArgument as Vector4? ?? Vector4.zero),
                EventArgumentType.Color   => EditorGUILayout.ColorField("Argument", _eventArgument as Color? ?? Color.white),
                EventArgumentType.Rect    => EditorGUILayout.RectField("Argument", _eventArgument as Rect? ?? new Rect()),
                EventArgumentType.Bounds  => EditorGUILayout.BoundsField("Argument", _eventArgument as Bounds? ?? new Bounds()),
                EventArgumentType.None    => null,
                _                         => throw new ArgumentOutOfRangeException()
            };
            if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                scriptableObjectEvent.Invoke(_sender, _eventArgument);
            }
        }
    }
    #endif

    #endregion
}