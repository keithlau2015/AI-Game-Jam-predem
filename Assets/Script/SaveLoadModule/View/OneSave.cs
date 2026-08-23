using SaveLoadModule;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class OneSave : MonoBehaviour
{
    [SerializeField]
    private Text acctID;
    [SerializeField]
    private Text progress;
    [SerializeField]
    private Text lastUpdateDate;
    [SerializeField]
    private Button button;

    public void SetUp(SaveSlotInfo slot, Action<SaveSlotInfo> cb)
    {
        if (slot == null)
            return;

        acctID.text = slot.DisplayName;
        progress.text = string.IsNullOrEmpty(slot.LastLevelKey) ? "" : slot.LastLevelKey;
        lastUpdateDate.text = slot.UpdatedUtc.ToString(CultureInfo.InvariantCulture);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => cb?.Invoke(slot));
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
