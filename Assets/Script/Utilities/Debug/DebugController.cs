
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class DebugController : Singleton<DebugController>
{
    private const int MAX_LOG_SIZE = 50;

    private Queue<string> commandLog = new Queue<string>(new string[] { "..." });
    public static string log = "...";
    public int tabValue = -1;

    public enum CommandID : int
    {
        CreateUI = 0, //param 1: UI prefab Key, param 2: parent
        SetSceneGameObjectActive = 1, //param 1: gameobject name, param 2: is active
        Clear = 2,
        StartBattle = 3,
    }

    public enum LogType : int
    {
        Command = 0,
        UnityDebug = 1,
    }

    public Dictionary<int, Func<string[], UniTask<bool>>> commandMap { get; private set; } = new Dictionary<int, Func<string[], UniTask<bool>>> ()
    {
        { (int)CommandID.CreateUI, CreateUI },
        { (int)CommandID.SetSceneGameObjectActive, SetSceneGameObjectActive },
        { (int)CommandID.Clear, ResetCommand},
        { (int)CommandID.StartBattle, StartBattle},
    };

    private static async UniTask<bool> CreateUI(string[] param)
    {
        if (param.Length < 2)
            return false;

        Transform parent = UIManager.viewLayer;
        GameObject parentGO = GameObject.Find(param[1]);
        if (parent == null)
            parent = parentGO.transform;
        await UIManager.singleton.LoadUI<Transform>(param[0], parent);
        return true;
    }

    private static async UniTask<bool> SetSceneGameObjectActive(string[] param)
    {
        if (param.Length < 2)
            return false;

        bool active = false;
        bool.TryParse(param[1], out active);

        bool breakpoint = false;
        GameObject go = null;
        while (!breakpoint)
        {
            go = GameObject.Find(param[0]);
            if (go != null)
                breakpoint = true;
            await UniTask.NextFrame();
        }
        if (go == null)
            return false;

        go.SetActive(active);

        return true;
    }

    private static async UniTask<bool> ResetCommand(string[] param)
    {
        log = "";
        await UniTask.NextFrame();
        return true;
    }

    private static async UniTask<bool> StartBattle(string[] param)
    {
        if (param.Length < 2)
            return false;

        int battleID = -1;
        int.TryParse(param[1], out battleID);
        if(battleID == -1)
            return false;

        //await BattleController.singleton.GenerateBattle(battleID);
        
        return true;
    }

    public async void ExecuteCommand(string value)
    {
        string[] splits = value.Split(' ');
        if (splits.Length < 1)
        {
            AddCommandLog(value);
            return;
        }

        CommandID id;
        if (!Enum.TryParse(splits[0], out id))
        {
            AddCommandLog(value);
            return;
        }

        Func<string[], UniTask<bool>> action = null;
        if (!commandMap.TryGetValue((int)id, out action))
        {
            AddCommandLog($"Execute {value} Failed, Invaild Command ID");
            return;
        }
        bool result = await action.Invoke(splits);
        string status = "fail";
        if (result)
            status = "success";

        AddCommandLog($"Execute {value} {status}");
    }

    public string Tab(string value = "")
    {
        if(value == string.Empty || value == "")
        {
            foreach(string name in Enum.GetNames(typeof(CommandID)))
            {
                if(name.Contains(value))
                {
                    CommandID id;
                    if (!Enum.TryParse(name, out id))
                        continue;

                    if (tabValue > (int)id && tabValue < Enum.GetNames(typeof(CommandID)).Length - 1)
                        continue;

                    else if(tabValue > Enum.GetNames(typeof(CommandID)).Length - 1)
                    {
                        tabValue = 0;
                    }
                    tabValue = (int)id;
                    return name + " ";
                }
            }

            return "";
        }
        else
        {
            tabValue++;
            return Enum.GetName(typeof(CommandID), tabValue) + " ";
        }
    }

    private void AddCommandLog(string value)
    {
        if (commandLog.Peek() == "...")
            commandLog.Enqueue(value);
        else
        {
            if(commandLog.Count > MAX_LOG_SIZE)
            {
                commandLog.Dequeue();
                commandLog.Enqueue(value);
            }
            else
                commandLog.Enqueue($"\n{value}");
        }

        UpdateLog();
    }

    public void UpdateLog(LogType type = LogType.Command)
    {
        log = "";
        if(type == LogType.Command)
        {
            bool isFirst = true;
            foreach (string value in commandLog)
            {
                if (isFirst)
                {
                    isFirst = false;
                    log += value;
                }
                else
                    log += "\n" + value;
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/NPI/GetObjectPath")]
    public static void GetGameObjectFullPath()
    {
        Transform target = null;
        if (Selection.transforms.Length > 0)
            target = Selection.transforms[0];
        else
            return;
        string path = target.name;
        while (target.parent != null)
        {
            target = target.parent;
            path = target.name + "/" + path;
        }
        Debug.Log(path);
    }
#endif
}
