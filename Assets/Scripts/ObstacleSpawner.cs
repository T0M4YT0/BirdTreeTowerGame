using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Spawning")]
    [SerializeField] private float obstacleSpacing = 4.5f;   // vertical distance between obstacles
    [SerializeField] private float spawnAhead = 30f;          // spawn this far above player
    [SerializeField] private float despawnBelow = 20f;         // destroy this far below player

    [Header("Gap")]
    [SerializeField] private float gapSize = 3.5f;           // vertical gap height (start)
    [SerializeField] private float minGapSize = 2.0f;         // smallest gap at max difficulty
    [SerializeField] private float difficultyRampHeight = 200f;
    [SerializeField] private float maxGapShift = 2.5f;        // how far the gap centre can move between obstacles

    [Header("Barrier")]
    [SerializeField] private float barrierWidth = 5f;         // wider than play area to be safe
    [SerializeField] private float barrierBlockHeight = 12f;   // height of each solid block

    [Header("Biome Colours")]
    [SerializeField] private Color cityColour = new Color(0.35f, 0.35f, 0.4f);
    [SerializeField] private Color treesColour = new Color(0.2f, 0.45f, 0.15f);
    [SerializeField] private Color skyColour = new Color(0.6f, 0.65f, 0.75f);
    [SerializeField] private Color stormColour = new Color(0.25f, 0.22f, 0.3f);
    [SerializeField] private Color spaceColour = new Color(0.5f, 0.7f, 0.9f);

    private float nextSpawnY;
    private float lastGapCentreY;

    private static Sprite _square;

    private void Start()
    {
        nextSpawnY = 8f;
        lastGapCentreY = 4f; // roughly where the bird starts
    }

    private void Update()
    {
        if (player == null) return;

        float ceiling = player.position.y + spawnAhead;
        while (nextSpawnY < ceiling)
        {
            SpawnObstacle(nextSpawnY);
            nextSpawnY += obstacleSpacing;
        }

        Cleanup();
    }

    private void SpawnObstacle(float y)
    {
        // Difficulty progress
        float progress = Mathf.Clamp01(y / difficultyRampHeight);
        float gap = Mathf.Lerp(gapSize, minGapSize, progress);

        // Shift the gap centre randomly but constrained
        float shift = Random.Range(-maxGapShift, maxGapShift);
        float gapCentre = lastGapCentreY + shift;

        // Keep gap reachable (within a reasonable band around current height)
        // The gap centre should be near the obstacle's Y position
        float minCentre = y - 2f;
        float maxCentre = y + 2f;
        gapCentre = Mathf.Clamp(y + shift * 0.3f, minCentre, maxCentre);
        lastGapCentreY = gapCentre;

        float gapBottom = gapCentre - gap / 2f;
        float gapTop = gapCentre + gap / 2f;

        Color colour = GetBiomeColour(y);

        // Parent object
        GameObject obstacle = new GameObject("Obstacle");
        obstacle.transform.position = new Vector3(0f, y, 0f);

        // Bottom block (below the gap)
        float bottomBlockTop = gapBottom;
        float bottomBlockCentreY = bottomBlockTop - barrierBlockHeight / 2f;
        CreateBlock(obstacle.transform, bottomBlockCentreY, colour);

        // Top block (above the gap)
        float topBlockBottom = gapTop;
        float topBlockCentreY = topBlockBottom + barrierBlockHeight / 2f;
        CreateBlock(obstacle.transform, topBlockCentreY, colour);

        // Score trigger in the gap
        GameObject scoreTrigger = new GameObject("ScoreZone");
        scoreTrigger.tag = "ScoreZone";
        scoreTrigger.transform.SetParent(obstacle.transform);
        scoreTrigger.transform.position = new Vector3(0f, gapCentre, 0f);
        BoxCollider2D scoreCol = scoreTrigger.AddComponent<BoxCollider2D>();
        scoreCol.size = new Vector2(barrierWidth, gap * 0.3f);
        scoreCol.isTrigger = true;
    }

    private void CreateBlock(Transform parent, float centreY, Color colour)
    {
        GameObject block = new GameObject("Block");
        block.transform.SetParent(parent);
        block.transform.position = new Vector3(0f, centreY, 0f);
        block.tag = "Obstacle";

        SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquare();
        sr.color = colour;
        sr.sortingLayerName = "Obstacles";
        block.transform.localScale = new Vector3(barrierWidth, barrierBlockHeight, 1f);

        BoxCollider2D col = block.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    private Color GetBiomeColour(float altitude)
    {
        if (altitude < 30f) return cityColour;
        if (altitude < 45f) return Color.Lerp(cityColour, treesColour, (altitude - 30f) / 15f);
        if (altitude < 75f) return treesColour;
        if (altitude < 90f) return Color.Lerp(treesColour, skyColour, (altitude - 75f) / 15f);
        if (altitude < 130f) return skyColour;
        if (altitude < 145f) return Color.Lerp(skyColour, stormColour, (altitude - 130f) / 15f);
        if (altitude < 200f) return stormColour;
        if (altitude < 215f) return Color.Lerp(stormColour, spaceColour, (altitude - 200f) / 15f);
        return spaceColour;
    }

    private void Cleanup()
    {
        float destroyBelow = player.position.y - despawnBelow;
        // Find root obstacle objects by name
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            if (obj.transform.parent == null && obj.name == "Obstacle"
                && obj.transform.position.y < destroyBelow)
            {
                Destroy(obj);
            }
        }
    }

    public void ResetAll()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            if (obj.transform.parent == null && obj.name == "Obstacle")
                Destroy(obj);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ScoreZone"))
            Destroy(obj);

        nextSpawnY = 8f;
        lastGapCentreY = 4f;
    }

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
