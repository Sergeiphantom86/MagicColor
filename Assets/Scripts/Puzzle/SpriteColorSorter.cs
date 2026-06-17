using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteColorSorter : MonoBehaviour
{
    private const float IgnoredTransparency = 0.1f;

    [SerializeField] private Sprite[] _sprites;

    private void Start()
    {
        SortAndPrint();
    }

    private void SortAndPrint()
    {
        var result = _sprites
            .Where(sprite => sprite != null)
            .Select(sprite => new
            {
                Name = sprite.name,
                ColorCount = CountColors(sprite.texture)
            })
            .OrderBy(data => data.ColorCount)
            .ToList();

        foreach (var item in result)
        {
            Debug.Log($"{item.Name} — {item.ColorCount} colors");
        }
    }

    private int CountColors(Texture2D texture)
    {
        if (texture == null)
            return 0;

        HashSet<Color32> colors = new HashSet<Color32>();

        Color32[] pixels = texture.GetPixels32();

        foreach (var pixel in pixels)
        {
            if (pixel.a >= IgnoredTransparency * 255)
            {
                colors.Add(pixel);
            }
        }

        return colors.Count;
    }
}