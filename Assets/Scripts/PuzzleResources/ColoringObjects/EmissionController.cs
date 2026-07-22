using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public sealed class EmissionController
    {
        private static readonly int EmissionColorId =
            Shader.PropertyToID("_EmissionColor");

        private static readonly int EmissionIntensityId =
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
                EmissionIntensityId,
                Mathf.Clamp01(intensity));

            _material.SetColor(
                EmissionColorId,
                emissionColor * Mathf.Clamp01(brightness));
        }

        public void Disable()
        {
            _material.DisableKeyword("_EMISSION");
        }
    }
}