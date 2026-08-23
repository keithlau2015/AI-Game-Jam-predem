using AttributeModule;
using LocalizationModule;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [SerializeField]
    private Text label;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private float smoothTime = 0.2f;
    private AttributeData attributeInstance;
    private float targetSliderValue;

    public void SetUp(AttributeData attributeInstance)
    {
        if (this.attributeInstance != null)
            this.attributeInstance.onValuePostChange -= UpdateValue;

        this.attributeInstance = attributeInstance;
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        attributeInstance.onValuePostChange += UpdateValue;
        ApplySliderValue(GetSliderRatio(attributeInstance.value, attributeInstance.maxValue), true);
        if (label != null)
            label.text = $"{LocalizationManager.singleton.GetLocalization("Attribute_T_0")}: {attributeInstance.value}";
    }

    private void Update()
    {
        if (slider == null)
            return;

        float delta = targetSliderValue - slider.value;
        if (Mathf.Abs(delta) <= 0.001f)
        {
            slider.value = targetSliderValue;
            return;
        }

        float t = 1f - Mathf.Exp(-Time.deltaTime / smoothTime);
        slider.value = Mathf.Lerp(slider.value, targetSliderValue, t);
    }

    private void UpdateValue(int dir, BigInteger diff, BigInteger value, BigInteger maxValue)
    {
        if (label != null)
            label.text = $"{LocalizationManager.singleton.GetLocalization("Attribute_T_0")}: {value}";

        ApplySliderValue(GetSliderRatio(value, maxValue), false);
    }

    private static float GetSliderRatio(BigInteger value, BigInteger maxValue)
    {
        if (maxValue <= 0)
            return 0f;

        return Mathf.Clamp01((float)value / (float)maxValue);
    }

    private void ApplySliderValue(float ratio, bool immediate)
    {
        targetSliderValue = ratio;
        if (slider == null)
            return;

        if (immediate)
            slider.value = ratio;
    }

    private void OnDestroy()
    {
        if (attributeInstance != null)
            attributeInstance.onValuePostChange -= UpdateValue;
        attributeInstance = null;
        if (label != null)
            label.text = "";
    }
}
