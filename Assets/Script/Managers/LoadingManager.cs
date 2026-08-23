using Cysharp.Threading.Tasks;
using GameUI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class LoadingManager : Singleton<LoadingManager>
{
    public enum PresentType : int
    {
        none = -1,
        ShowPercentage = 0,
        ShowProgress = 1,
        ShowSize = 2,
    }

    [SerializeField]
    private LoadingPanel loadingPanel;
    [SerializeField]
    private GameObject miniLoadingPanel;
    private ObjectPool<GameObject> miniLoadingPool;

    private static Queue tasks = new Queue();
    public bool isLoading
    {
        get
        {
            return loadingPanel.gameObject.activeInHierarchy && tasks.Count > 0;
        }
    }

    private class LoadingProcedure
    {
        private string name;
        private int totalValue;
        private int currentValue;
        public PresentType presentType;
        public Progress<int> progress { get; private set; }
        public LoadingProcedure(LoadingPanel loadingPanel, PresentType presentType, string name, int totalValue)
        {
            this.name = name;
            this.totalValue = totalValue;
            this.currentValue = 0;
            this.presentType = presentType;
            loadingPanel.SetUpSubProgressBar(totalValue, 0);
            progress = new Progress<int>();
            progress.ProgressChanged += (e, s) => {
                this.currentValue = s;
                loadingPanel.OnSubProgressChange(s, GetProgressLabel());
                if (s >= totalValue)
                    OnSubProcedureDone();
            };
        }

        private string GetProgressLabel()
        {
            string result = "";
            if(presentType == PresentType.ShowPercentage)
            {
                float percentage = ((float)currentValue / (float)totalValue) * (float)100;
                result = $"{name} [{Mathf.RoundToInt(percentage)}%]";
            }
            else if (presentType == PresentType.ShowProgress)
            {
                result = $"{name} [{currentValue}/{totalValue}]";
            }
            else if (presentType == PresentType.ShowSize)
            {
                result = name + ": " + FileManager.SizeSuffix((currentValue / totalValue));
            }
            return result;
        }

        private void OnSubProcedureDone()
        {
            if (tasks.Count > 0) tasks.Dequeue();
            progress = null;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Application.lowMemory += ReleaseAllGameObject;
    }

    public async UniTask<IProgress<int>> AddTask(PresentType presentType, string procedureName, int totalCount)
    {
        await UniTask.WaitUntil(() => tasks.Count == 0);
        if (loadingPanel == null || totalCount <= 0)
        {
            loadingPanel.OnTaskFinish();
            return null;
        }

        loadingPanel.ResetSubProgressBar();
        LoadingProcedure newLoadingProcedure = new LoadingProcedure(loadingPanel, presentType, procedureName, totalCount);
        tasks.Enqueue(newLoadingProcedure);
        return newLoadingProcedure.progress;
    }

    public void Show(bool showSumUpProgress, int totalTask)
    {
        if (loadingPanel == null)
            return;
        loadingPanel.Show(showSumUpProgress, totalTask);
    }

    public void Hide()
    {
        if (loadingPanel == null)
            return;
        loadingPanel.Hide();
    }

    public GameObject ShowMini(Transform parent = null)
    {
        if(miniLoadingPool == null)
        {
            miniLoadingPool = new ObjectPool<GameObject>(
              () => Instantiate(miniLoadingPanel.gameObject),
              o => o.SetActive(true),
              o =>
              {
                  o.transform.SetParent(transform);
                  o.SetActive(false);
              });
        }

        GameObject go = miniLoadingPool.Get();
        if (parent)
        {
            go.transform.SetParent(parent);
            RectTransform rectTransform = null;
            if(go.TryGetComponent(out rectTransform))
            {
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
            }
        }

        return go;
    }

    public void HideMini(GameObject gameObject)
    {
        miniLoadingPool.Release(gameObject);
    }

    public void Reset()
    {
        //loadingPanel.ResetSubProgressBar();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        loadingPanel = null;
        Application.lowMemory -= ReleaseAllGameObject;
    }

    private void ReleaseAllGameObject()
    {
        miniLoadingPool.Clear();
    }
}
