using System;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Pause / engine entry point that owns the RTS GameStateMachine.
/// Lives with the state machine under RTSModule/Input.
/// </summary>
public class GameStateController : Singleton<GameStateController>
{
    private GameStateMachine _stateMachine;
    public GameStateMachine stateMachine
    {
        get
        {
            if (_stateMachine == null)
            {
                GameObject go = new GameObject("stateMachine");
                go.transform.SetParent(transform, false);
                _stateMachine = go.AddComponent<GameStateMachine>();
            }

            return _stateMachine;
        }
    }

    public bool IsPause { get; private set; }
    public bool IsBattleActive { get { return stateMachine != null && stateMachine.isActiveAndEnabled; } }
    public event Action<bool> onPause;

    public void BattlePause()
    {
        IsPause = !IsPause;
        onPause?.Invoke(IsPause);
    }

    public void SetPaused(bool paused)
    {
        if (IsPause == paused)
            return;
        IsPause = paused;
        onPause?.Invoke(IsPause);
    }

    public static void DestoryAllChild(GameObject root)
    {
        if (root == null)
            return;
        for (int i = 0; i < root.transform.childCount; i++)
        {
            Destroy(root.transform.GetChild(i).gameObject);
        }
    }

    public static void ChangeAllLayer(GameObject root, string layerName)
    {
        if (root == null)
            return;
        int layer = LayerMask.NameToLayer(layerName);
        root.layer = layer;
        for (int i = 0; i < root.transform.childCount; i++)
        {
            root.transform.GetChild(i).gameObject.layer = layer;
        }
    }

    public void InitializeEngine()
    {
        SetPaused(false);
        stateMachine.SetState(new GameEngineInitState(stateMachine));
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

#if UNITY_EDITOR
    private static void AddDefinition(string definition)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definitions + ";" + definition);
    }

    private static void RemoveDefinition(string definition)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var splited = definitions.Split(';');

        var builder = new StringBuilder();
        var addedCount = 0;
        foreach (var item in splited)
        {
            if (item == definition)
                continue;

            builder.Append(item);
            builder.Append(';');
            addedCount++;
        }

        if (addedCount != 0)
            builder.Remove(builder.Length - 1, 1);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, builder.ToString());
    }
#endif
}
