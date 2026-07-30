using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public sealed class EmissionController
    {
        private static readonly int s_emissionColorId =
            Shader.PropertyToID("_EmissionColor");

        private static readonly int s_emissionIntensityId =
            Shader.PropertyToID("_EmissionIntensity");

        private readonly Material _material;

        public EmissionController(Material material)
        {
            _material = material;
        }

        public void Enable(Color emissionColor,
                           float intensity = 0.01f,
                           float brightness = 0.5f)
        {
            _material.EnableKeyword("_EMISSION");

            _material.SetFloat(
                s_emissionIntensityId,
                Mathf.Clamp01(intensity));

            _material.SetColor(
                s_emissionColorId,
                emissionColor * Mathf.Clamp01(brightness));
        }

        public void Disable()
        {
            _material.DisableKeyword("_EMISSION");
        }
    }
}