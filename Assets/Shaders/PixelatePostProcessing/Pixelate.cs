using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Pixelate : VolumeComponent, IPostProcessComponent
{
    [Range(0, 100), Tooltip("0=disable")]
    public IntParameter colorStep = new IntParameter(6);
    [Tooltip("0=disable")]
    public IntParameter downscaleResolutionX = new IntParameter(1080);

    public bool IsActive() => colorStep != 0 && downscaleResolutionX != 0;

    public bool IsTileCompatible() => false;
}