using UnityEngine;

public interface ISelectable : IHoverable
{
    public void OnSelect();
    public void OnDeselect();
}