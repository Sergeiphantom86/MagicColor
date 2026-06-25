using System.Collections;
using UnityEngine;
namespace Shader
{

public class SpriteGlowToggle : MonoBehaviour
{
    //private static readonly int GlowPowerID = Shader.PropertyToID("_GlowPower");

    [SerializeField] private GameObject _spriteObject;

    public bool GlowEnabled = true;
    public float GlowPowerOn = 3f;
    public float GlowPowerOff = 0f;

    private Material runtimeMat;

    private void Awake()
    {
        if (_spriteObject == null)
            return;

        if (_spriteObject.TryGetComponent<SpriteRenderer>(out var sr) == false)
        {
            Debug.LogError("SpriteGlowToggle: No SpriteRenderer on assigned GameObject!");
            return;
        }

        runtimeMat = Instantiate(sr.material);
        sr.material = runtimeMat;

        Apply();
    }

    private IEnumerator Start()
    {
        EnableGlow();

        yield return new WaitForSeconds(2);

        DisableGlow();
    }

    public void EnableGlow()
    {
        GlowEnabled = true;

        Apply();
    }

    public void DisableGlow()
    {
        GlowEnabled = false;

        Apply();
    }

    private void Apply()
    {
        //if (runtimeMat == null)
        //    return;
        //runtimeMat.SetFloat(GlowPowerID, GlowEnabled ? GlowPowerOn : GlowPowerOff);
    }

    private void OnDestroy()
    {
        if (runtimeMat != null) Destroy(runtimeMat);
    }
}

}