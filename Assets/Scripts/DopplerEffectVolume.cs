// Scripts/DopplerEffectVolume.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Effects/Relativistic Doppler")]
public class DopplerEffectVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter beta = new ClampedFloatParameter(0f, -0.99f, 0.99f);
    public ClampedFloatParameter intensity = new ClampedFloatParameter(1f, 0f, 5f);

    public bool IsActive() => intensity.value > 0f;
    public bool IsTileCompatible() => false;
}