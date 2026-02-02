using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private const float GradientStart = 0f;
    private const float GradientEnd = 1f;
    private const float FullyOpaque = 1f;
    private const float FullyTransparent = 0f;

    [SerializeField] ParticleSystem[] _monochrome;
    [SerializeField] TrailRenderer _gradient;

    private void Start()
    {
        SetRedColor();
    }

    public void SetGreenColor()
    {
        ChangeParticleColor(Color.green);
        ChangeTrailColor(Color.green);
    }

    private void SetRedColor()
    {
        ChangeParticleColor(Color.red);
        ChangeTrailColor(Color.red);
    }

    private void ChangeParticleColor(Color newColor)
    {
        if (_monochrome == null) return;

        Gradient gradient = CreateColorToWhiteGradient(newColor);

        foreach (ParticleSystem particleSystem in _monochrome)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = new ParticleSystem.MinMaxGradient(gradient);
            }
        }
    }

    private void ChangeTrailColor(Color startColor)
    {
        if (_gradient != null)
        {
            Gradient gradient = CreateColorToWhiteGradient(startColor);
            _gradient.colorGradient = gradient;
        }
    }

    private Gradient CreateColorToWhiteGradient(Color color)
    {
        return CreateGradient(
            CreateColorKey(color, GradientStart),
            CreateColorKey(Color.white, GradientEnd),
            CreateAlphaKey(color.a, GradientStart),
            CreateAlphaKey(FullyTransparent, FullyOpaque)
        );
    }

    private Gradient CreateGradient(GradientColorKey startColorKey, GradientColorKey endColorKey,GradientAlphaKey startAlphaKey, GradientAlphaKey endAlphaKey)
    {
        Gradient gradient = new()
        {
            colorKeys = new GradientColorKey[] { startColorKey, endColorKey },
            alphaKeys = new GradientAlphaKey[] { startAlphaKey, endAlphaKey }
        };

        return gradient;
    }

    private GradientColorKey CreateColorKey(Color color, float time)
    {
        return new GradientColorKey(color, time);
    }

    private GradientAlphaKey CreateAlphaKey(float alpha, float time)
    {
        return new GradientAlphaKey(alpha, time);
    }
}