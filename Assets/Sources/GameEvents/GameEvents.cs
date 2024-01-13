using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GameEvents
{
    public static ScriptableObjectEvent OnPause =
        Resources.Load<ScriptableObjectEvent>("GameEvents/Pause");

    public static ScriptableObjectEvent OnGameOver =
        Resources.Load<ScriptableObjectEvent>("GameEvents/GameOver");

    public static ScriptableObjectEvent OnRestart =
        Resources.Load<ScriptableObjectEvent>("GameEvents/Restart");
}