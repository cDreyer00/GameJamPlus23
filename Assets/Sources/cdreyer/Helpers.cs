using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public static class Helpers
{
    static Camera cam;
    public static Camera mainCamera
    {
        get
        {
            if (cam == null)
            {
                cam = Camera.main;
            }
            return cam;
        }
    }

    static readonly Dictionary<float, WaitForSeconds> waitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        if (waitDictionary.TryGetValue(time, out var wait)) return wait;
        waitDictionary[time] = new WaitForSeconds(time);
        return waitDictionary[time];
    }

    private static PointerEventData eventDataCurrentPosition;
    static List<RaycastResult> results;
    public static bool IsOverUI
    {
        get
        {
            eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }

    public static void DeleteAllChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    public static List<Transform> GetAllChildren(Transform transform, List<Transform> children = null)
    {
        if (children == null) children = new();
        if (transform.childCount < 1) return children;

        foreach (Transform child in transform)
        {
            children.Add(child);

            GetAllChildren(child, children);
        }

        return children;
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    #region Delay

    class AsyncHolder : MonoBehaviour { }
    static AsyncHolder _asyncHolder;

    public static void DelayFrames(int frames, Action action)
    {
        if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();

        _asyncHolder.StartCoroutine(C(action, frames));

        static IEnumerator C(Action e, int frames)
        {
            for (; frames > 0; frames--)
                yield return null; // wait a frame loop

            e?.Invoke();
        }
    }
    public static void DelayFrames<TState>(int frames, Action<TState> action, TState state)
    {
        if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();

        _asyncHolder.StartCoroutine(C(frames, action, state));

        static IEnumerator C(int frames, Action<TState> e, TState state)
        {
            for (; frames > 0; frames--)
                yield return null; // wait a frame loop

            e?.Invoke(state);
        }
    }

    public static void Delay(float secs, Action action)
    {
        if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();

        _asyncHolder.StartCoroutine(C(action, secs));
        return;

        static IEnumerator C(Action action, float secs)
        {
            yield return GetWait(secs);
            action?.Invoke();
        }
    }
    public static void Delay<TState>(this TState m, float secs, Action<TState> action)
        where TState : MonoBehaviour
    {
        if (m == null || m.IsDestroyed() || !m.isActiveAndEnabled) return;

        m.StartCoroutine(C(secs, action, m));
        return;

        static IEnumerator C(float secs, Action<TState> action, TState state)
        {
            yield return GetWait(secs);
            action?.Invoke(state);
        }
    }
    public static void Delay<TState>(float secs, Action<TState> action, TState state)
    {
         if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();
         
         _asyncHolder.StartCoroutine(C(secs, action, state));
         return;

         static IEnumerator C(float secs, Action<TState> action, TState state)
         {
             yield return GetWait(secs);
             action?.Invoke(state);
         }
    }
    public static void Repeat<TState>(float delay, float period, Action<TState> action, TState state)
    {
        if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();

        _asyncHolder.StartCoroutine(C(delay, period, action, state));
        return;

        static IEnumerator C(float delay, float period, Action<TState> action, TState state)
        {
            yield return GetWait(delay);
            while (true)
            {
                action?.Invoke(state);
                yield return GetWait(period);
            }
        }
    }
    public static void WaitUntil(Func<bool> predicate, Action action)
    {
        if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();

        _asyncHolder.StartCoroutine(C(action, predicate));
        return;

        static IEnumerator C(Action action, Func<bool> predicate)
        {
            yield return new WaitUntil(predicate);
            action?.Invoke();
        }
    }
    
    public static void WaitUntil<TState>(this TState state, Func<TState,bool> predicate, Action<TState> action)
        where TState : MonoBehaviour
    {
        state.StartCoroutine(C(action, predicate, state));
        return;

        static IEnumerator C(Action<TState> action, Func<TState,bool> predicate, TState state)
        {
            while (!predicate(state))
                yield return null;
            action?.Invoke(state);
        }
    }
    
    public static void OnComplete(IEnumerator coroutine, Action action)
    {
        if (_asyncHolder == null)
            _asyncHolder = new GameObject("Async_Holder").AddComponent<AsyncHolder>();

        _asyncHolder.StartCoroutine(C(action, coroutine));
        return;

        static IEnumerator C(Action action, IEnumerator coroutine)
        {
            yield return coroutine;
            action?.Invoke();
        }
    } 
    public static void OnComplete<TState>(this TState state, IEnumerator coroutine, Action<TState> action)
        where TState : MonoBehaviour
    {
        state.StartCoroutine(C(action, coroutine, state));
        return;

        static IEnumerator C(Action<TState> action, IEnumerator coroutine, TState state)
        {
            yield return coroutine;
            action?.Invoke(state);
        }
    }

    #endregion

    public static void ChangeObjectLayer(Transform tr, int layer)
    {
        tr.gameObject.layer = layer;
        Transform[] childrens = tr.GetComponentsInChildren<Transform>();
        foreach (var child in childrens) child.gameObject.layer = layer;
    }

    public static bool TryFindObjectOfType<T>(out T type) where T : MonoBehaviour
    {
        type = GameObject.FindObjectOfType<T>();
        return type != null;
    }

    public static T GetRandom<T>(this IEnumerable<T> list)
    {
        if (list == null || list.Count() == 0) return default;
        return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
    }
}

public static class JsonHelper
{
    public static T FromJson<T>(string json) => JsonUtility.FromJson<T>(json);

    public static string ToJson<T>(T data) => JsonUtility.ToJson(data);

    /// <summary>
    /// usage: YourType[] objects = JsonHelper.FromJsonArray<YourType>(jsonString);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T[] FromJsonArray<T>(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static string ToJsonArray<T>(T[] array)
    {
        Wrapper<T> wrapper = new()
        {
            array = array
        };

        return JsonUtility.ToJson(wrapper);
    }

    class Wrapper<T>
    {
        public T[] array;
    }
}