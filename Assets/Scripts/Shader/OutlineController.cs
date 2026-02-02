using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial;
    private Material originalMaterial;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
    }

    public void SetSelected(bool selected)
    {
        rend.material = selected ? outlineMaterial : originalMaterial;
    }
}