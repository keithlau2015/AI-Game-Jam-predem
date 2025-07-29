using Cinemachine;
using UnityEngine;

public class CinemachineShake : Singleton<CinemachineShake>
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private float shakeTimer = 0;
    private float startingIntensity = 0;
    private float shakeTimerTotal = 0;

    public void ShakeCamera(float intensity, float time)
    {
        cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
    }
}
