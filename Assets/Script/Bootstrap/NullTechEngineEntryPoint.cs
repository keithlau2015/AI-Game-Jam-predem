using UnityEngine;

/// <summary>
/// Scene entry component for Bootloader.unity. Kicks off engine initialization.
/// GUID must stay e2c112e487904cb4a959e174a1bad57e (wired in Bootloader scene).
/// </summary>
public class NullTechEngineEntryPoint : MonoBehaviour
{
    private void Awake()
    {
        GameStateController.singleton.InitializeEngine();
    }
}
