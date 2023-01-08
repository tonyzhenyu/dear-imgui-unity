using UnityEngine.Rendering;
using UnityEngine;

#if !HAS_URP
namespace ImGuiNET.Unity
{
    public class RenderImGuiFeature : UnityEngine.ScriptableObject
    {
        public CommandBuffer commandBuffer;
    }
}
#else
using UnityEngine.Rendering.Universal;

namespace ImGuiNET.Unity
{
    public class RenderImGuiFeature : ScriptableRendererFeature
    {
        class ExecuteCommandBufferPass : ScriptableRenderPass
        {
            public CommandBuffer cmd;
            public CameraType FilterType { get; set; }
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (((int)renderingData.cameraData.cameraType & (int)FilterType) <= 0)
                {
                    return;
                }
                context.ExecuteCommandBuffer(cmd);
            }
        }

        ExecuteCommandBufferPass _executeCommandBufferPass;

        public CommandBuffer commandBuffer;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        public override void Create()
        {
            _executeCommandBufferPass = new ExecuteCommandBufferPass()
            {
                cmd = commandBuffer,
                renderPassEvent = renderPassEvent,
                FilterType = CameraType.Game
                };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (commandBuffer == null) return;
            _executeCommandBufferPass.renderPassEvent = renderPassEvent;
            _executeCommandBufferPass.cmd = commandBuffer;
            renderer.EnqueuePass(_executeCommandBufferPass);
        }
    }
}
#endif
