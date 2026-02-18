using System.Collections;
using UnityEngine;

public class SpriteGlowToggle : MonoBehaviour
{
    [SerializeField] private GameObject _spriteObject;
    public bool glowEnabled = true;
    public float glowPowerOn = 3f;
    public float glowPowerOff = 0f;

    private Material runtimeMat;
    private static readonly int GlowPowerID = Shader.PropertyToID("_GlowPower"); // <-- здесь

    void Awake()
    {
        if (_spriteObject == null) return;

        SpriteRenderer sr = _spriteObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SpriteGlowToggle: No SpriteRenderer on assigned GameObject!");
            return;
        }

        // создаём инстанс материала, чтобы не ломать другие объекты
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

    public void EnableGlow() { glowEnabled = true; Apply(); }
    public void DisableGlow() { glowEnabled = false; Apply(); }
    public void ToggleGlow() { glowEnabled = !glowEnabled; Apply(); }

    private void Apply()
    {
        if (runtimeMat == null) return;
        runtimeMat.SetFloat(GlowPowerID, glowEnabled ? glowPowerOn : glowPowerOff);
    }

    void OnDestroy()
    {
        if (runtimeMat != null) Destroy(runtimeMat);
    }
}
