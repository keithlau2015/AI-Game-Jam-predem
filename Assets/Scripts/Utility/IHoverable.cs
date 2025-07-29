using UnityEngine;

public interface IHoverable
{
    public void OnHoverIn();
    public void OnHoverOut();
    public GameObject GetGameObject();
}
