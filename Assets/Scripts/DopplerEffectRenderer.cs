using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DopplerEffectRenderer : ScriptableRendererFeature
{
    class DopplerEffectPass : ScriptableRenderPass
    {
        private Material material;
        private DopplerEffectVolume volume;
        private TextureHandle cameraColorTexture;

        public DopplerEffectPass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetVolume(DopplerEffectVolume vol)
        {
            volume = vol;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null || volume == null || !volume.IsActive())
                return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            cameraColorTexture = resourceData.activeColorTexture;

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Doppler Effect Pass", out var passData))
            {
                passData.material = material;
                passData.beta = volume.beta.value;
                passData.intensity = volume.intensity.value;
                passData.cameraColorTexture = cameraColorTexture;

                builder.UseTexture(cameraColorTexture, AccessFlags.ReadWrite);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    data.material.SetFloat("_Beta", data.beta);
                    data.material.SetFloat("_Intensity", data.intensity);
                    
                    // Correct URP 17.0.3 blit implementation
                    RasterCommandBuffer cmd = context.cmd;
                    Blitter.BlitTexture(
                        cmd,
                        data.cameraColorTexture,
                        new Vector4(1, 1, 0, 0),
                        data.material,
                        0
                    );
                });
            }
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {}
    }

    private DopplerEffectPass dopplerPass;
    private Material material;

    public override void Create()
    {
        if (material == null)
            material = CoreUtils.CreateEngineMaterial("Custom/DopplerPostProcess");
        
        dopplerPass = new DopplerEffectPass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.postProcessEnabled)
        {
            var stack = VolumeManager.instance.stack;
            dopplerPass.SetVolume(stack.GetComponent<DopplerEffectVolume>());
            renderer.EnqueuePass(dopplerPass);
        }
    }

    class PassData
    {
        public Material material;
        public float beta;
        public float intensity;
        public TextureHandle cameraColorTexture;
    }
}