using Cysharp.Threading.Tasks;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region reference
    [SerializeField]
    private Transform refTopLayer;
    [SerializeField]
    private Transform refViewLayer;
    [SerializeField]
    private Canvas refViewLayerCanvas;
    [SerializeField]
    private Transform refBaseLayer;
    [SerializeField]
    private CommonPopTextPanel refCommonPopUpTextPanel;
    [SerializeField]
    private LiteDescriptionPanel refLiteDescriptionPanel;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [SerializeField]
    private DebugPanel refDebugPanel;
    [SerializeField]
    private Button showDebugPanel;
#endif
#endregion

    public static Transform topLayer { get; private set; }
    public static Transform viewLayer { get; private set; }
    public static Transform baseLayer { get; private set; }

    private Stack<IPreviousablePanel> previousablePanels = new Stack<IPreviousablePanel>();

    //Default Common Text Pop Up Panel Config
    private CommonPopTextPanel.CommonPopUpTextPanelConfig defaultConfig = new CommonPopTextPanel.CommonPopUpTextPanelConfig() {
            showGreenBtn = true,
            greenBtnLabeID = "SYS_Confirm",
            showRedBtn = true,
            redBtbLabelID = "SYS_Cacnel"
    };

    protected override void Awake()
    {
        base.Awake();

        topLayer = refTopLayer;
        viewLayer = refViewLayer;
        baseLayer = refBaseLayer;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (refDebugPanel != null)
            showDebugPanel.onClick.AddListener(() => ShowDebugPanel(!refDebugPanel.gameObject.activeSelf));
#endif
        InputManager.singleton.AddBackAction(() => { PreviousPage(); }, true);
        if (!InputManager.singleton.playerControl.UI.Point.enabled)
            InputManager.singleton.playerControl.UI.Point.Enable();
        if (!InputManager.singleton.playerControl.UI.Click.enabled)
            InputManager.singleton.playerControl.UI.Click.Enable();
    }

    public async UniTask<T> LoadUI<T>(string key, Transform parent = null)
    {
        //By Default Set Panel to View Layer
        if (parent == null)
            parent = viewLayer;
        AsyncOperationHandle asyncInstantiateOP = Addressables.InstantiateAsync(key, parent);
        await asyncInstantiateOP;
        if (asyncInstantiateOP.IsDone)
        {
            GameObject go = asyncInstantiateOP.Result as GameObject;
            T result;
            if (!go.TryGetComponent(out result))
            {
                Debug.LogError($"LoadUI: Cannot get component {typeof(T)}");
            }
            IPreviousablePanel previousablePanel = null;
            if (go.TryGetComponent(out previousablePanel))
                previousablePanels.Push(previousablePanel);

            return result;
        }
        return default;
    }

    public async void LoadUI(string key, Transform parent = null)
    {
        //By Default Set Panel to View Layer
        if (parent == null)
            parent = viewLayer;
        AsyncOperationHandle asyncInstantiateOP = Addressables.InstantiateAsync(key, parent);
        await asyncInstantiateOP;
        if (asyncInstantiateOP.IsDone)
        {
            GameObject go = asyncInstantiateOP.Result as GameObject;
            IPreviousablePanel previousablePanel = null;
            if (go.TryGetComponent(out previousablePanel))
                previousablePanels.Push(previousablePanel);
        }
    }

    public IPreviousablePanel PreviousPage()
    {
        if (previousablePanels == null || previousablePanels.Count == 0)
            return null;

        previousablePanels.Pop().Hide();

        if (previousablePanels.Count <= 0)
            return null;

        IPreviousablePanel currentPanel = previousablePanels.Peek();
        return currentPanel;
    }

    public IPreviousablePanel GetTopPanel()
    {
        if (previousablePanels == null || previousablePanels.Count == 0)
            return null;

        IPreviousablePanel currentPanel = null;
        previousablePanels.TryPeek(out currentPanel);
        return currentPanel;
    }

    public void RemoveTopPreviousPanel()
    {
        if (previousablePanels == null || previousablePanels.Count == 0)
            return;

        previousablePanels.Pop();
    }


    public void ShowCommonPopUpTextPanel(bool show, CommonPopTextPanel.CommonPopUpTextPanelConfig conifg = default, string content = "", Action onGreenBtnClickCB = null, Action onRedBtnClickCB = null)
    {
        if (refCommonPopUpTextPanel == null)
            return;
        if (show)
        {
            previousablePanels.Push(refCommonPopUpTextPanel);
            refCommonPopUpTextPanel.Show(conifg, content, onGreenBtnClickCB, onRedBtnClickCB);
        }
        else
        {
            previousablePanels.Pop();
            refCommonPopUpTextPanel.Hide();
        }
    }

    public void ShowLiteDescription(bool show, string localizationID = "")
    {
        if (refLiteDescriptionPanel == null)
            return;

        if (show)
        {
            refLiteDescriptionPanel.Show(localizationID);
        }
        else
        {
            refLiteDescriptionPanel.Hide();
        }
    }

    public async void ClearAllUI()
    {
        if (refLiteDescriptionPanel.gameObject.activeInHierarchy)
            refLiteDescriptionPanel.Hide();

        foreach (IPreviousablePanel previousablePanel in previousablePanels)
        {
            previousablePanel.Hide();
        }

        previousablePanels.Clear();

        await new WaitUntil(() =>  { return previousablePanels.Count == 0; });
        GameManager.DestoryAllChild(refViewLayer.gameObject);
        await new WaitUntil(() => { return refViewLayer.childCount == 0; });
        GameManager.DestoryAllChild(refBaseLayer.gameObject);
        await new WaitUntil(() => { return refBaseLayer.childCount == 0; });
    }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void ShowDebugPanel(bool show)
    {
        if (refDebugPanel == null)
            return;

        if(show)
        {
            previousablePanels.Push(refDebugPanel);
            refDebugPanel.Show();
        }
        else
        {
            previousablePanels.Pop();
            refDebugPanel.Hide();
        }
    }
#endif
    protected override void OnDestroy()
    {
        previousablePanels.Clear();
        base.OnDestroy();
    }

    public bool IsPanelExists(Type panelType)
    {
        var panels = refViewLayer.GetComponentsInChildren(panelType);
        return panels.Length > 0;
    }

    public T[] GetExistPanel<T>()
    {
        return refViewLayer.GetComponentsInChildren<T>();
    }

    public bool IsAnyPanelExist()
    {
        return viewLayer.childCount > 0;
    }

    public float GetViewLayerSacleFactor()
    {
        if (!refViewLayerCanvas)
            return 1;

        return refViewLayerCanvas.scaleFactor;
    }
}
