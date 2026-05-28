using System.Collections.Generic;
using UnityEngine;

public class BackgroundTiller : MonoBehaviour
{
    [Header("refs")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform leftWallPos;
    [SerializeField] private Transform rightWallPos;

    [Header("settings")]
    [SerializeField] private float wallThickness = 0.35f;
    [SerializeField] private float spawnAhead = 30f;
    [SerializeField] private float despawnBelow = 20f;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private int sortingOrder = -1;

    private readonly List<GameObject> bgTiles = new List<GameObject>();
    private float tileWorldW;
    private float tileWorldH;
    private float centerX;
    private float nextSpawnY;
    private bool ready = false;

    private void Start()
    {
        if (backgroundSprite == null || player == null)
            return;
        if (!CalculateDimensions())
            return;

        nextSpawnY = player.position.y - despawnBelow;
        ready = true;
    }

    private void Update()
    {
        if (!ready || player == null)
            return;

        float ceiling = player.position.y + spawnAhead;
        if (nextSpawnY < ceiling)
        {
            SpawnTile(nextSpawnY);
            nextSpawnY += tileWorldH;
        }

        float destroyBelow = player.position.y - despawnBelow;
        for (int i = bgTiles.Count - 1; i >= 0; i--)
        {
            if (bgTiles[i] == null || bgTiles[i].transform.position.y < destroyBelow)
            {
                if (bgTiles[i] != null) Destroy(bgTiles[i]);
                bgTiles.RemoveAt(i);
            }
        }
    }

    private bool CalculateDimensions()
    {
        float lx = leftWallPos.position.x + wallThickness * 0.5f;
        float rx = rightWallPos.position.x - wallThickness * 0.5f;
        tileWorldW = rx - lx;
        centerX = (lx + rx) * 0.5f;

        float ppu = backgroundSprite.pixelsPerUnit;
        float spriteW = backgroundSprite.rect.width / ppu;
        float spriteH = backgroundSprite.rect.height / ppu;

        if (spriteW <= 0f || tileWorldW <= 0f)
        {
            Debug.LogError("bgTiller - invalid dimensions");
            return false;
        }

        tileWorldH = tileWorldW * (spriteH / spriteW);

        if (tileWorldH <= 0f)
        {
            Debug.LogError("bgTiller - tileWorldH is zero");
            return false;
        }

        Debug.Log($"bgTiller - tileW={tileWorldW:F2} tileH={tileWorldH:F2}");
        return true;
    }

    private void SpawnTile(float y)
    {
        GameObject tile = new GameObject("BgTile");
        tile.transform.SetParent(transform, false);

        var sr = tile.AddComponent<SpriteRenderer>();
        sr.sprite = backgroundSprite;
        sr.color = Color.white;
        sr.sortingOrder = sortingOrder;

        float ppu = backgroundSprite.pixelsPerUnit;
        float spriteW = backgroundSprite.rect.width / ppu;
        float spriteH = backgroundSprite.rect.height / ppu;

        tile.transform.localScale = new Vector3(tileWorldW / spriteW, tileWorldH / spriteH, 1f);

        tile.transform.position = new Vector3(centerX, y + tileWorldH * 0.5f, 1f);
        bgTiles.Add(tile);
    }

    public void ResetTiler()
    {
        foreach (var t in bgTiles)
            if (t != null) Destroy(t);
        bgTiles.Clear();
        ready = false;

        if (backgroundSprite == null || player == null) return;
        if (!CalculateDimensions()) return;

        nextSpawnY = player.position.y - despawnBelow;
        ready = true;
    }
}