using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelatePass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "Render Pixelate Effect";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelate");
    static readonly int ColorStepId = Shader.PropertyToID("_ColorStep");
    Pixelate pixelate;
    Material pixelateMaterial;
    RenderTargetIdentifier currentTarget;

    public PixelatePass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        var shader = Shader.Find("G.H.S/Pixelate");
        if (shader == null)
        {
            Debug.LogError("Shader not found.");
            return;
        }
        pixelateMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (pixelateMaterial == null)
        {
            Debug.LogError("Material not created.");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled) return;

        var stack = VolumeManager.instance.stack;
        pixelate = stack.GetComponent<Pixelate>();
        if (pixelate == null) { return; }
        if (!pixelate.IsActive()) { return; }

        var cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;
        var source = currentTarget;
        int destination = TempTargetId;

        int w = pixelate.downscaleResolutionX.value;
        if (w == 0) w = cameraData.camera.scaledPixelWidth;
        var h = Mathf.RoundToInt(cameraData.camera.aspect * w);
        pixelateMaterial.SetInt(ColorStepId, pixelate.colorStep.value);
        //pixelateMaterial.SetInt(FocusDetailId, zoomBlur.focusDetail.value);
        //zoomBlurMaterial.SetVector(FocusScreenPositionId, zoomBlur.focusScreenPosition.value);
        //zoomBlurMaterial.SetInt(ReferenceResolutionXId, zoomBlur.referenceResolutionX.value);

        int shaderPass = 0;
        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, pixelateMaterial, shaderPass);
    }
}