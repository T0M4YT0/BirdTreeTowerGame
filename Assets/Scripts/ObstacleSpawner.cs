using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Play Area")]
    [SerializeField] private float playAreaHalfWidth = 1.905f;
    [SerializeField] private Transform leftWallPos, rightWallPos;

    [Header("walls and platform settings")]
    [SerializeField] private float platformThickness = 0.35f;
    [SerializeField] private float wallThickness = 0.35f;
    [SerializeField] private int wallSegCount = 20;
    [SerializeField] private float wallSegH;

    [Header("Spawning")]
    [SerializeField] private float platformSpacing = 5f;
    [SerializeField] private float spawnAhead = 30f;
    [SerializeField] private float despawnBelow = 20f;

    [Header("Difficulty")]
    [SerializeField] private float difficultyRampHeight = 200f;

    [Header("Biome Colours")]
    [SerializeField] private Color cityColour = new Color(0.35f, 0.35f, 0.4f);
    [SerializeField] private Color treesColour = new Color(0.2f, 0.45f, 0.15f);
    [SerializeField] private Color skyColour = new Color(0.6f, 0.65f, 0.75f);
    [SerializeField] private Color stormColour = new Color(0.25f, 0.22f, 0.3f);
    [SerializeField] private Color spaceColour = new Color(0.5f, 0.7f, 0.9f);

    [Header("sprites")]
    [SerializeField] private Sprite wallSprite;
    [SerializeField] private Sprite platformSprite;

    //two segments per side
    private GameObject[] leftSegs, rightSegs;
    private SpriteRenderer[] leftSRs, rightSRs;

    private float nextSpawnY;
    private readonly List<GameObject> platforms = new List<GameObject>();

    private static Sprite _square;

    private void Start()
    {
        nextSpawnY = 10f;
        BuildWalls();
    }

    private void Update()
    {
        if (player == null) return;

        //keep spawning until filled up to spawn ahead dist
        float ceiling = player.position.y + spawnAhead;
        while (nextSpawnY < ceiling)
        {
            SpawnPlatform(nextSpawnY);
            nextSpawnY += platformSpacing;
        }

        //resue wall segments
        UpdateWalls();

        //destroy platforms far enough below player
        float destroyBelow = player.position.y - despawnBelow;
        for (int i = platforms.Count - 1; i >= 0; i--)
        {
            if (platforms[i] == null || platforms[i].transform.position.y < destroyBelow)
            {
                if (platforms[i] != null) Destroy(platforms[i]);
                platforms.RemoveAt(i);
            }
        }
    }

    //walls
    private void BuildWalls()
    {
        leftSegs = new GameObject[wallSegCount];
        rightSegs = new GameObject[wallSegCount];
        leftSRs = new SpriteRenderer[wallSegCount];
        rightSRs = new SpriteRenderer[wallSegCount];

        float lx = leftWallPos.position.x;
        float rx = rightWallPos.position.x;

        for (int i = 0; i < wallSegCount; i++)
        {
            float y = i * wallSegH;
            leftSegs[i] = MakeWallSeg("WallL_" + i, lx, y);
            leftSegs[i].tag = "Wall";
            rightSegs[i] = MakeWallSeg("WallR_" + i, rx, y);
            rightSegs[i].tag = "Wall";
            leftSRs[i] = leftSegs[i].GetComponent<SpriteRenderer>();
            rightSRs[i] = rightSegs[i].GetComponent<SpriteRenderer>();
        }
    }

    private GameObject MakeWallSeg(string name, float x, float y)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector3(x, y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = wallSprite != null ? wallSprite : GetSquare();
        sr.color = cityColour;
        sr.sortingLayerName = "Obstacles";

        //scale sprite fills to wall thickness
        Sprite s = sr.sprite;
        float ppu = s.pixelsPerUnit;
        float spriteWorldW = s.rect.width / ppu;
        float spriteWorldH = s.rect.height / ppu;

        go.transform.localScale = new Vector3(wallThickness / spriteWorldW, wallSegH / spriteWorldH, 1f);

        var col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(spriteWorldW, spriteWorldH);

        return go;
    }

    private void UpdateWalls()
    {
        for (int i = 0; i < wallSegCount; i++)
        {
            ScrollSeg(leftSegs[i], leftSRs[i]);
            ScrollSeg(rightSegs[i], rightSRs[i]);
        }
    }

    private void ScrollSeg(GameObject seg, SpriteRenderer sr)
    {
        //reset to top of stack when drops below
        float top = seg.transform.position.y + wallSegH * 0.5f;
        if (top < player.position.y - despawnBelow)
        {
            seg.transform.position = new Vector3(seg.transform.position.x, seg.transform.position.y + wallSegH * wallSegCount, 0f);

            //update physics
            Physics2D.SyncTransforms();
        }

        //recolour to match biome
        sr.color = GetBiomeColour(seg.transform.position.y);
    }

    //platforms
    private void SpawnPlatform(float y)
    {
        float progress = Mathf.Clamp01(y / difficultyRampHeight);

        bool fromLeft = Random.value < 0.5f;

        float lx = leftWallPos.position.x + wallThickness * 0.5f;//inner left
        float rx = rightWallPos.position.x - wallThickness * 0.5f;//inner right
        float innerWidth = rx - lx;//width between walls

        float minW = Mathf.Lerp(0.4f, 0.6f, progress) * (innerWidth * 0.5f);
        float maxW = Mathf.Lerp(0.6f, 0.9f, progress) * (innerWidth * 0.5f);
        float w = Random.Range(minW, maxW);

        // flush to the inner edge of the chosen wall
        float x = fromLeft ? lx + w * 0.5f : rx - w * 0.5f;

        Color colour = GetBiomeColour(y);

        GameObject p = new GameObject("Platform");
        p.transform.position = new Vector3(x, y, 0f);
        p.transform.localScale = new Vector3(w, platformThickness, 1f);
        p.tag = "Obstacle";

        var sr = p.AddComponent<SpriteRenderer>();
        sr.sprite = platformSprite != null ? platformSprite : GetSquare();
        sr.color = colour;
        sr.sortingLayerName = "Obstacles";

        var col = p.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;

        // score zone in the open gap
        float gapW = innerWidth - w;
        float gapX = fromLeft ? rx - gapW * 0.5f : lx + gapW * 0.5f;

        GameObject zone = new GameObject("ScoreZone");
        zone.tag = "ScoreZone";
        zone.transform.SetParent(transform, false);
        zone.transform.position = new Vector3(gapX, y, 0f);
        var zCol = zone.AddComponent<BoxCollider2D>();
        zCol.size = new Vector2(gapW * 0.8f, platformThickness);
        zCol.isTrigger = true;
        zone.transform.AddComponent<ScoreTrigger>();

        platforms.Add(p);
    }

    //biome colour
    private Color GetBiomeColour(float alt)
    {
        if (alt < 30f) return cityColour;
        if (alt < 45f) return Color.Lerp(cityColour, treesColour, (alt - 30f) / 15f);
        if (alt < 75f) return treesColour;
        if (alt < 90f) return Color.Lerp(treesColour, skyColour, (alt - 75f) / 15f);
        if (alt < 130f) return skyColour;
        if (alt < 145f) return Color.Lerp(skyColour, stormColour, (alt - 130f) / 15f);
        if (alt < 200f) return stormColour;
        if (alt < 215f) return Color.Lerp(stormColour, spaceColour, (alt - 200f) / 15f);
        return spaceColour;
    }

    // reset
    public void ResetAll()
    {
        foreach (var p in platforms)
            if (p != null) Destroy(p);
        platforms.Clear();

        nextSpawnY = 10f;

        float lx = leftWallPos.position.x;
        float rx = rightWallPos.position.x;

        for (int i = 0; i < wallSegCount; i++)
        {
            float y = i * wallSegH;
            leftSegs[i].transform.position = new Vector3(lx, y, 0f);
            rightSegs[i].transform.position = new Vector3(rx, y, 0f);
        }

        Physics2D.SyncTransforms();
    }

    //sprite
    public static Sprite GetSquare()
    {
        if (_square != null) return _square;
        Texture2D tex = new Texture2D(4, 4);
        Color[] px = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        _square = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        return _square;
    }
}