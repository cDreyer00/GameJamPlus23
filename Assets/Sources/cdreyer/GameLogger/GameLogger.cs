using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLogger : Singleton<GameLogger>
{
    [SerializeField] UILog logPrefab;
    [SerializeField] Transform logsParent;
    [SerializeField] List<Log> logs = new();
    [SerializeField] ButtonBehaviour hideOrShowButton;

    bool isActive = false;
    void Start()
    {
        hideOrShowButton.AddListener(() =>
        {
            isActive = !isActive;
            logsParent.parent.gameObject.SetActive(isActive);
        },
        InteractionType.ClickUp);
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogRecieved;
        Debug.Log("Logger enabled");
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogRecieved;
    }

    void LogRecieved(string message, string stackTrace, LogType type)
    {
        Log l = new(message, stackTrace, type);
        logs.Add(l);
        UILog uiLog = Instantiate(logPrefab, logsParent);
        uiLog.Init(l);
    }

    static string curCode = "";
    public static void Log(object message, string color = "white", string code = "")
    {
        if (!Debug.isDebugBuild) return;

        if (curCode != "")
            if (code != curCode) return;

        Debug.Log($"<color={color}>{message}</color>");
    }
}

[Serializable]
public class Log
{
    public string message;
    public string[] traceMessages;
    public LogType type;

    public Log(string message, string stackTrace, LogType type)
    {
        this.message = message;

        traceMessages = stackTrace.Split(
            new string[] { "\n" },
            StringSplitOptions.RemoveEmptyEntries
        );
        Array.Reverse(traceMessages);

        this.type = type;
    }
}