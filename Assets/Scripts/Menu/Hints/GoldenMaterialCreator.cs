using UnityEngine;

public class GoldenMaterialCreator : MonoBehaviour
{
    [Header("Golden Material Settings")]
    [ColorUsage(true, true)]
    public Color goldenColor = new Color(0.83f, 0.68f, 0.21f);
    [Range(0, 1)] public float metallic = 0.9f;
    [Range(0, 1)] public float smoothness = 0.85f;

    [Header("References")]
    public Renderer targetRenderer;

    [ContextMenu("Apply Golden Material")]

    private void Start()
    {
        ApplyGoldenMaterial();
    }
    public void ApplyGoldenMaterial()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        Material goldenMat = CreateGoldenMaterial();
        targetRenderer.material = goldenMat;
    }

    public Material CreateGoldenMaterial()
    {
        // Определяем шейдер в зависимости от пайплайна
        string shaderName = "Standard";

#if UNITY_URP
        shaderName = "Universal Render Pipeline/Lit";
#elif UNITY_HDRP
        shaderName = "HDRP/Lit";
#endif

        Material material = new Material(Shader.Find(shaderName));
        material.color = goldenColor;
        material.SetFloat("_Metallic", metallic);

        // Разные имена для smoothness в разных пайплайнах
        if (shaderName.Contains("Standard"))
            material.SetFloat("_Glossiness", smoothness);
        else
            material.SetFloat("_Smoothness", smoothness);

        material.name = "GoldenMaterial";
        return material;
    }
}