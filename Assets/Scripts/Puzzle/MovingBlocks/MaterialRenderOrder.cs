using UnityEngine;

public class MaterialRenderOrder : MonoBehaviour
{
    [Header("Настройки рендер очереди")]
    public int inFrontRenderQueue = 2000;  // Когда куб перед забором
    public int behindRenderQueue = 1000;   // Когда куб за забором

    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material clonedMaterial;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // Клонируем материал чтобы не менять оригинальный
            originalMaterial = objectRenderer.material;
            clonedMaterial = new Material(originalMaterial);
            objectRenderer.material = clonedMaterial;
        }
    }

    /// <summary>
    /// Установить куб ПЕРЕД забором
    /// </summary>
    public void SetInFrontOfFence()
    {
        if (clonedMaterial != null)
            clonedMaterial.renderQueue = inFrontRenderQueue;
    }

    /// <summary>
    /// Установить куб ЗА забором
    /// </summary>
    public void SetBehindFence()
    {
        if (clonedMaterial != null)
            clonedMaterial.renderQueue = behindRenderQueue;
    }

    void OnDestroy()
    {
        // Восстанавливаем оригинальный материал
        if (objectRenderer != null && originalMaterial != null)
            objectRenderer.material = originalMaterial;
    }
}