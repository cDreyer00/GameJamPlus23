using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
        foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
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
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static async void ActionCallback(Action action, float timer)
    {
        bool playing = Application.isPlaying;

        await Task.Delay(TimeSpan.FromSeconds(timer));

        if (playing != Application.isPlaying)
        {
            Debug.LogWarning("ActionCallback cancelled bacause playmode changed");
            return;
        }

        action?.Invoke();
    }

    public static void ChangeObjectLayer(Transform tr, int layer)
    {
        tr.gameObject.layer = layer;
        Transform[] childrens = tr.GetComponentsInChildren<Transform>();
        foreach (var child in childrens) child.gameObject.layer = layer;
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