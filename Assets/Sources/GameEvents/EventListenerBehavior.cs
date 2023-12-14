using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EventListenerBehavior : MonoBehaviour
{
    public ScriptableObjectEvent eventSource;
    public UnityEventResponse    eventResponse;
    void OnEnable()
    {
        if (eventSource != null) {
            eventSource.listeners.Add(this);
        }
    }
    void OnDisable()
    {
        if (eventSource != null) {
            eventSource.listeners.Remove(this);
        }
    }
    public void OnInvoke(Object sender, object args)
    {
        eventResponse.Invoke(sender, args);
    }
    public static implicit operator EventListener(EventListenerBehavior listenerBehavior)
    {
        return new(listenerBehavior.eventSource, listenerBehavior.eventResponse);
    }

    #region Editor

    #if UNITY_EDITOR
    [CustomEditor(typeof(EventListenerBehavior))]
    public class EventListenerBehaviorEditor : Editor
    {
        enum EventArgumentType
        {
            None, Object, Int,
            Float, String, Bool,
            Vector2, Vector3, Vector4,
            Color, Rect, Bounds,
        }
        EventArgumentType _eventArgumentType;
        object            _eventArgument;
        Object            _sender;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var eventListenerBehavior = (EventListenerBehavior)target;
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
                eventListenerBehavior.OnInvoke(_sender, _eventArgument);
            }
        }
    }
    #endif

    #endregion
}