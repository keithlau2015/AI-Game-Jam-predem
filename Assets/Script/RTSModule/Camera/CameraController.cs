using Cinemachine;
using UnityEngine;
public class CameraController : Singleton<CameraController>
{
    [SerializeField]
    private Camera base3DCam, baseUICam;

    public Camera getBase3DCam() { return base3DCam; }
    public Camera GetBaseUICam() { return baseUICam; }
}