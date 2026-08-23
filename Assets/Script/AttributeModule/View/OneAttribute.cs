using AttributeModule;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class OneAttribute : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text label;

    public int attributeType;
    private AttributeData attributeInstance;
    public async void SetUp(int type, AttributeData attributeInstance)
    {
        this.attributeInstance = attributeInstance;
        this.attributeType = type;
        this.attributeInstance.onValuePostChange += UpdateLabel;
        Sprite sprite = await AssetsBundleManager.LoadSprite($"AttributeType_{attributeType}");
        if (icon)
            icon.sprite = sprite;
        UpdateLabel(0, 0, this.attributeInstance.value, this.attributeInstance.maxValue);
        gameObject.SetActive(true);
    }

    private void UpdateLabel(int dir, BigInteger diff, BigInteger value, BigInteger maxValue)
    {
        if (this.attributeInstance == null)
            return;

        if (this.label == null)
            return;

        label.text = $"{this.attributeInstance.value}";
    }

    private void OnDestroy()
    {
        this.attributeInstance.onValuePostChange -= UpdateLabel;
    }
}
