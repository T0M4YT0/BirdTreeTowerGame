using UnityEngine;

public class BiomeBackground : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform player;

    // Biome colours
    private static readonly Color city  = new Color(0.12f, 0.12f, 0.18f);
    private static readonly Color trees = new Color(0.08f, 0.22f, 0.08f);
    private static readonly Color sky   = new Color(0.3f, 0.5f, 0.8f);
    private static readonly Color storm = new Color(0.08f, 0.08f, 0.12f);
    private static readonly Color space = new Color(0.01f, 0.01f, 0.04f);

    private struct Biome { public float start; public Color colour; }
    private readonly Biome[] biomes = {
        new Biome { start = 0f,   colour = city },
        new Biome { start = 30f,  colour = trees },
        new Biome { start = 75f,  colour = sky },
        new Biome { start = 130f, colour = storm },
        new Biome { start = 200f, colour = space },
    };

    private void Update()
    {
        if (player == null || cam == null) return;
        float alt = player.position.y;

        // Find current biome pair and blend
        for (int i = biomes.Length - 1; i >= 0; i--)
        {
            if (alt >= biomes[i].start)
            {
                if (i == biomes.Length - 1)
                {
                    cam.backgroundColor = biomes[i].colour;
                }
                else
                {
                    float t = Mathf.Clamp01((alt - biomes[i].start) / (biomes[i + 1].start - biomes[i].start));
                    cam.backgroundColor = Color.Lerp(biomes[i].colour, biomes[i + 1].colour, t);
                }
                return;
            }
        }
        cam.backgroundColor = city;
    }
}
