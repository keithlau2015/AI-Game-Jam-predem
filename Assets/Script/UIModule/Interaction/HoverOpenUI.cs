using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOpenUI : HoverDescription
{
    [SerializeField]
    private string uiPanelTypeName;
    public override void OnHoverIn()
    {
        base.OnHoverIn();
    }

    public override void OnHoverOut()
    {
        base.OnHoverOut();
    }

    public override void OnClick()
    {
        Type uiPanelType = Type.GetType(uiPanelTypeName);
        if (UIManager.singleton.IsPanelExists(uiPanelType))
        {
            return;
        }

        UIManager.singleton.LoadUI(uiPanelType.Name);
    }
}
