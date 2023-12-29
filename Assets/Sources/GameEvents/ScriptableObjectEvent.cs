using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ScriptableObjectEvent", menuName = "ScriptableObjectEvent")]
public class ScriptableObjectEvent : ScriptableObject
{
    public InlinedArray<EventListener> listeners;
    public void Invoke<TObject, TArgs>(TObject sender, TArgs args)
        where TObject : Object
    {
        for (int i = listeners.length - 1; i >= 0; i--) {
            listeners[i].OnInvoke(sender, args);
        }
    }
    public void AddListener(EventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Append(listener);
    }
    public bool RemoveListener(EventListener listener) => listeners.Remove(listener);
    public void RemoveAllListeners() => listeners.Clear();
    public void AddListener<TSender, TArgs>(Action<TSender, TArgs> listener)
        where TSender : Object
    {
        var eventListener = new EventListener();
        eventListener.SetEvent(listener);
        if (!listeners.Contains(eventListener))
            listeners.Append(eventListener);
    }
    public void AddListener(Action listener)
    {
        var eventListener = new EventListener();
        eventListener.SetEvent(listener);
        if (!listeners.Contains(eventListener))
            listeners.Append(eventListener);
    }
    public bool RemoveListener(Action listener)
    {
        var eventListener = new EventListener();
        eventListener.SetEvent(listener);
        return listeners.Remove(eventListener);
    }
    public bool RemoveListener<TSender, TArgs>(Action<TSender, TArgs> listener)
        where TSender : Object
    {
        var eventListener = new EventListener();
        eventListener.SetEvent(listener);
        return listeners.Remove(eventListener);
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
            switch (_eventArgumentType) {
            case EventArgumentType.Object:
                _eventArgument = EditorGUILayout.ObjectField("Argument", (Object)_eventArgument, typeof(Object), true);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Object)_eventArgument);
                }
                break;
            case EventArgumentType.Int:
                _eventArgument = EditorGUILayout.IntField("Argument", _eventArgument as int? ?? 0);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (int)_eventArgument);
                }
                break;
            case EventArgumentType.Float:
                _eventArgument = EditorGUILayout.FloatField("Argument", _eventArgument as float? ?? 0f);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (float)_eventArgument);
                }
                break;
            case EventArgumentType.String:
                _eventArgument = EditorGUILayout.TextField("Argument", _eventArgument as string ?? string.Empty);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (string)_eventArgument);
                }
                break;
            case EventArgumentType.Bool:
                _eventArgument = EditorGUILayout.Toggle("Argument", _eventArgument as bool? ?? false);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (bool)_eventArgument);
                }
                break;
            case EventArgumentType.Vector2:
                _eventArgument = EditorGUILayout.Vector2Field("Argument", _eventArgument as Vector2? ?? Vector2.zero);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Vector2)_eventArgument);
                }
                break;
            case EventArgumentType.Vector3:
                _eventArgument = EditorGUILayout.Vector3Field("Argument", _eventArgument as Vector3? ?? Vector3.zero);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Vector3)_eventArgument);
                }
                break;
            case EventArgumentType.Vector4:
                _eventArgument = EditorGUILayout.Vector4Field("Argument", _eventArgument as Vector4? ?? Vector4.zero);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Vector4)_eventArgument);
                }
                break;
            case EventArgumentType.Color:
                _eventArgument = EditorGUILayout.ColorField("Argument", _eventArgument as Color? ?? Color.white);
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Color)_eventArgument);
                }
                break;
            case EventArgumentType.Rect:
                _eventArgument = EditorGUILayout.RectField("Argument", _eventArgument as Rect? ?? new Rect());
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Rect)_eventArgument);
                }
                break;
            case EventArgumentType.Bounds:
                _eventArgument = EditorGUILayout.BoundsField("Argument", _eventArgument as Bounds? ?? new Bounds());
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, (Bounds)_eventArgument);
                }
                break;
            case EventArgumentType.None:
                _eventArgument = null;
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Invoke")) {
                    scriptableObjectEvent.Invoke(_sender, _eventArgument);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endif

    #endregion
}