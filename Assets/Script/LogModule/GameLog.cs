using Cysharp.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Generic gameplay logger (LogModule). Writes timestamped lines via FileManager.
/// </summary>
public class GameLog
{
    private string logFileName = string.Empty;
    public event Action<string> onLogging;
    private static GameLog _log;
    private float sessionStartTime;

    public static GameLog logger
    {
        get
        {
            if (_log == null)
                _log = new GameLog();
            return _log;
        }
    }

    private GameLog()
    {
        sessionStartTime = Time.time;
        string stamp = TimeManager.singleton != null
            ? TimeManager.singleton.GetCurrentDatetime().ToString("ddMMyyyy")
            : DateTime.Now.ToString("ddMMyyyy");
        logFileName = $"GameLog_{stamp}";
    }

    public async void Log(string ctx)
    {
        if (!IsLoggingEnabled())
            return;
        ctx = $"[{GetFormattedSessionTime()}] {ctx}\n";
        await Write(ctx);
        Debug.Log(ctx);
        onLogging?.Invoke(ctx);
    }

    public async void Warning(string ctx)
    {
        if (!IsLoggingEnabled())
            return;
        ctx = $"[{GetFormattedSessionTime()}] (WARNING) {ctx}\n";
        await Write(ctx);
        Debug.LogWarning(ctx);
        onLogging?.Invoke(ctx);
    }

    public async void Error(string ctx)
    {
        if (!IsLoggingEnabled())
            return;
        ctx = $"[{GetFormattedSessionTime()}] (ERROR) {ctx}\n";
        await Write(ctx);
        Debug.LogError(ctx);
        onLogging?.Invoke(ctx);
    }

    private bool IsLoggingEnabled()
    {
        if (GameStateController.singleton == null)
            return true;
        return !GameStateController.singleton.IsPause;
    }

    private async UniTask Write(string ctx)
    {
        try
        {
            await FileManager.WriteFile<string>(
                FileManager.FileType.Log,
                Regex.Replace(ctx.Replace("\n", ""), "<.*?>", ""),
                logFileName,
                false);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"GameLog write failed: {e.Message}");
        }
    }

    private string GetFormattedSessionTime()
    {
        float elapsed = Time.time - sessionStartTime;
        if (GameStateController.singleton != null
            && GameStateController.singleton.stateMachine != null
            && GameStateController.singleton.stateMachine.curBattleTime > 0)
        {
            elapsed = GameStateController.singleton.stateMachine.curBattleTime;
        }

        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(elapsed));
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
