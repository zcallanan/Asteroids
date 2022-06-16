using UnityEngine;

namespace Utils
{
    public static class MaterialMode
    {
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Mode = Shader.PropertyToID("_Mode");

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public static void SetupBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetFloat(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetFloat(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetFloat(ZWrite, 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetFloat(Mode, 3.0f);
                    break;
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetFloat(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat(DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat(ZWrite, 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    material.SetFloat(Mode, 0.0f);
                    break;
                default:
                    Debug.Log($"Warning: BlendMode: {blendMode} is not yet implemented!");
                    break;
            }
        }
    }
}
