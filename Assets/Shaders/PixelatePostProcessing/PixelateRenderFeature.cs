using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateRenderFeature : ScriptableRendererFeature
{
    PixelatePass pixelatePass;

    public override void Create()
    {
        pixelatePass = new PixelatePass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pixelatePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pixelatePass);
    }
}