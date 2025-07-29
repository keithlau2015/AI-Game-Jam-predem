using System;
using EPOOutline;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class HoverDescription : MonoBehaviour, IHoverable
{
    public string localizationKey;
    private Outlinable outline;


    protected void Awake()
    {
        TryGetComponent(out outline);
    }

    public virtual void OnHoverIn()
    {
        if(outline)
            outline.enabled = true;

        UIManager.singleton.ShowLiteDescription(true, localizationKey);
    }

    public virtual void OnHoverOut()
    {
        if(outline)
            outline.enabled = false;

        UIManager.singleton.ShowLiteDescription(false);
    }

    public virtual void OnClick()
    {

    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
