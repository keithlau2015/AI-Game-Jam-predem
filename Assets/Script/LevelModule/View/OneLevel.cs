using Model;
using System;
using UnityEngine;
using UnityEngine.UI;

public class OneLevel : MonoBehaviour
{
    [SerializeField]
    private Text levelId, levelName;
    [SerializeField]
    private Button button;
    private LevelModel model;
    public void SetUp(LevelModel model, Action<LevelModel> cb)
    {
        this.model = model;
        levelId.text = model.key.ToString();
        levelName.text = model.name;
        button.onClick.AddListener(() => cb?.Invoke(model));
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}