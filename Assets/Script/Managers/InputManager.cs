using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private LinkedList<Action> onBackButtonAction;
    private List<Action> onPermanentBackAction;
    public class InputSetting
    {
        public float cursorSensitiveX;
        public float cursorSensitiveY;
        public InputSetting()
        {
            this.cursorSensitiveX = 1f;
            this.cursorSensitiveY = 1f;
        }
        public string keycodeBindings;
    }
    public PlayerControl playerControl;
    public InputSetting inputSettingConfig;

    public bool isInit { get; private set; } = false;

    public bool isMultiMode { get; private set; } = false;

    public void SetUp(List<InputSetting> list = null)
    {
        if (list == null || list.Count == 0)
        {
            this.inputSettingConfig = new InputSetting();
        }
        else
        {
            InputSetting inputSetting = list[0];
            if (inputSetting == null)
            {
                this.inputSettingConfig = new InputSetting();
            }
            else
                this.inputSettingConfig = inputSetting;
        }
        playerControl = new PlayerControl();
        if(!string.IsNullOrEmpty(this.inputSettingConfig.keycodeBindings))
            playerControl.LoadBindingOverridesFromJson(this.inputSettingConfig.keycodeBindings);
        playerControl.Enable();
        isInit = true;
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame && !LoadingManager.singleton.isLoading)
            TriggerBackAction();
    }

    protected override void Awake()
    {
        if (inputSettingConfig == null)
            inputSettingConfig = new InputSetting();
        if(playerControl == null)
            playerControl = new PlayerControl();

        playerControl.UI.Enable();
        playerControl.UI.MultiMode.Enable();
        playerControl.UI.MultiMode.performed += ctx => TriggerMultiMode();

        base.Awake();
    }

    protected override void OnDestroy()
    {
        if(playerControl.UI.MultiMode != null)
            playerControl.UI.MultiMode.performed -= ctx => TriggerMultiMode();
        base.OnDestroy();
        FileManager.SaveFile<InputSetting>(new InputSetting[] { this.inputSettingConfig }, FileManager.FileType.Save, "InputPref");
    }

    public void AddBackAction(Action action, bool isPermanent = false)
    {
        if (isPermanent)
        {
            if (onPermanentBackAction == null)
                onPermanentBackAction = new List<Action>();

            onPermanentBackAction.Add(action);
        }
        else
        {
            if (onBackButtonAction == null)
                onBackButtonAction = new LinkedList<Action>();

            onBackButtonAction.AddLast(action);
        }
    }

    public void RemoveBackAction(Action action)
    {
        if (action == null)
            return;

        if (onBackButtonAction.Find(action) != null)
        {
            onBackButtonAction.Remove(action);
        }
    }

    public void RemoveBackPermanentAction(Action action)
    {
        if(action == null)
            return;

        onPermanentBackAction?.Remove(action);
    }

    public void TriggerBackAction()
    {
        Action action = null;
        if (onBackButtonAction != null && onBackButtonAction.Count != 0)
        { 
            action = onBackButtonAction.Last.Value;
            action?.Invoke();
            onBackButtonAction.RemoveLast();
        }
        else
        {
            foreach(Action permanentAction in onPermanentBackAction)
            {
                permanentAction?.Invoke();
            }
        }
    }

    public bool IsBackActionsEmpty()
    {
        return onBackButtonAction != null && onPermanentBackAction != null && onBackButtonAction.Count == 0 && onPermanentBackAction.Count == 0;
    }

    public void SaveCurrentKeycodeBindings()
    {
        this.inputSettingConfig.keycodeBindings = playerControl.SaveBindingOverridesAsJson();
    }

    private void TriggerMultiMode()
    {
        if(isMultiMode)
            isMultiMode = false;
        else
            isMultiMode = true;
    }
}
