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
            bool removed = eventSource.listeners.Remove(this);
            Debug.Assert(removed, "Listener was not removed from event source.");
        }
    }
    public void OnInvoke<TSender, TArg>(TSender sender, TArg args)
        where TSender : Object
    {
        eventResponse.Invoke(sender, args);
    }
    public static implicit operator EventListener(EventListenerBehavior listenerBehavior)
    {
        var eventListener = new EventListener();
        eventListener.SetEvent(listenerBehavior.eventResponse);
        return eventListener;
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
            switch (_eventArgumentType) {
            case EventArgumentType.Object:
                _eventArgument = EditorGUILayout.ObjectField("Argument", (Object)_eventArgument, typeof(Object), true);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Object)_eventArgument);
                }
                break;
            case EventArgumentType.Int:
                _eventArgument = EditorGUILayout.IntField("Argument", _eventArgument as int? ?? 0);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (int)_eventArgument);
                }
                break;
            case EventArgumentType.Float:
                _eventArgument = EditorGUILayout.FloatField("Argument", _eventArgument as float? ?? 0f);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (float)_eventArgument);
                }
                break;
            case EventArgumentType.String:
                _eventArgument = EditorGUILayout.TextField("Argument", _eventArgument as string ?? string.Empty);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (string)_eventArgument);
                }
                break;
            case EventArgumentType.Bool:
                _eventArgument = EditorGUILayout.Toggle("Argument", _eventArgument as bool? ?? false);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (bool)_eventArgument);
                }
                break;
            case EventArgumentType.Vector2:
                _eventArgument = EditorGUILayout.Vector2Field("Argument", _eventArgument as Vector2? ?? Vector2.zero);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Vector2)_eventArgument);
                }
                break;
            case EventArgumentType.Vector3:
                _eventArgument = EditorGUILayout.Vector3Field("Argument", _eventArgument as Vector3? ?? Vector3.zero);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Vector3)_eventArgument);
                }
                break;
            case EventArgumentType.Vector4:
                _eventArgument = EditorGUILayout.Vector4Field("Argument", _eventArgument as Vector4? ?? Vector4.zero);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Vector4)_eventArgument);
                }
                break;
            case EventArgumentType.Color:
                _eventArgument = EditorGUILayout.ColorField("Argument", _eventArgument as Color? ?? Color.white);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Color)_eventArgument);
                }
                break;
            case EventArgumentType.Rect:
                _eventArgument = EditorGUILayout.RectField("Argument", _eventArgument as Rect? ?? new Rect());
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Rect)_eventArgument);
                }
                break;
            case EventArgumentType.Bounds:
                _eventArgument = EditorGUILayout.BoundsField("Argument", _eventArgument as Bounds? ?? new Bounds());
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    eventListenerBehavior.OnInvoke(_sender, (Bounds)_eventArgument);
                }
                break;
            case EventArgumentType.None:
                _eventArgument = null;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endif

    #endregion
}