using System;
using System.IO;
using UnityEditor;

public static class GameEvents
{
    const string _basePath = "Assets/Sources/GameEvents";

    public readonly static ScriptableObjectEvent OnPause =
        AssetDatabase.LoadAssetAtPath<ScriptableObjectEvent>(Path.Combine(_basePath, "Pause.asset"));

    public readonly static ScriptableObjectEvent OnGameOver =
        AssetDatabase.LoadAssetAtPath<ScriptableObjectEvent>(Path.Combine(_basePath, "GameOver.asset"));

    public readonly static ScriptableObjectEvent OnRestart =
        AssetDatabase.LoadAssetAtPath<ScriptableObjectEvent>(Path.Combine(_basePath, "Restart.asset"));
}