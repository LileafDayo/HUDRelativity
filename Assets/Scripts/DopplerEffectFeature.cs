using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DopplerEffectFeature : ScriptableRendererFeature
{
    class DopplerEffectPass : ScriptableRenderPass
    {
        private Material material;
        private DopplerEffectVolume volume;
        private TextureHandle cameraColor;
        private TextureHandle tempTexture;

        public DopplerEffectPass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void Setup(DopplerEffectVolume vol) => volume = vol;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null || volume == null || !volume.IsActive()) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            cameraColor = resourceData.activeColorTexture;

            var desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            // Create temporary texture using RenderGraph
            tempTexture = renderGraph.CreateTexture(new TextureDesc(desc.width, desc.height)
            {
                name = "_TempDopplerTexture",
                colorFormat = desc.graphicsFormat,
                clearBuffer = true,
                clearColor = Color.clear,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            });

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Doppler Effect", out var passData))
            {
                passData.material = material;
                passData.beta = volume.beta.value;
                passData.intensity = volume.intensity.value;
                passData.source = cameraColor;
                passData.dest = tempTexture;

                builder.UseTexture(cameraColor, AccessFlags.Read);
                builder.UseTexture(tempTexture, AccessFlags.Write);
                
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    data.material.SetFloat("_Beta", data.beta);
                    data.material.SetFloat("_Intensity", data.intensity);
                    
                    // Proper BlitTexture arguments
                    Blitter.BlitTexture(
                        context.cmd,
                        data.source,
                        Vector4.zero, // Replace with an appropriate Vector4 if needed
                        data.material,
                        0
                    );
                });
            }
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {}
    }

    class PassData
    {
        public Material material;
        public float beta;
        public float intensity;
        public TextureHandle source;
        public TextureHandle dest;
    }

    private DopplerEffectPass pass;
    private Material material;

    public override void Create()
    {
        material = CoreUtils.CreateEngineMaterial("Hidden/RelativisticDoppler");
        pass = new DopplerEffectPass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.postProcessEnabled)
        {
            var stack = VolumeManager.instance.stack;
            pass.Setup(stack.GetComponent<DopplerEffectVolume>());
            renderer.EnqueuePass(pass);
        }
    }
}