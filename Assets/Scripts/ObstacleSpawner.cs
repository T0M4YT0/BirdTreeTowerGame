using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Spawning")]
    [SerializeField] private float obstacleSpacing = 5f;
    [SerializeField] private float spawnAhead = 30f;
    [SerializeField] private float despawnBelow = 20f;

    [Header("Difficulty")]
    [SerializeField] private float difficultyRampHeight = 200f;

    [Header("Bar")]
    [SerializeField] private float playAreaHalfWidth = 1.905f;
    [SerializeField] private float barThickness = 0.35f;

    [Header("Biome Colours")]
    [SerializeField] private Color cityColour = new Color(0.35f, 0.35f, 0.4f);
    [SerializeField] private Color treesColour = new Color(0.2f, 0.45f, 0.15f);
    [SerializeField] private Color skyColour = new Color(0.6f, 0.65f, 0.75f);
    [SerializeField] private Color stormColour = new Color(0.25f, 0.22f, 0.3f);
    [SerializeField] private Color spaceColour = new Color(0.5f, 0.7f, 0.9f);

    private float nextSpawnY;
    private bool lastFromLeft = false;
    private static Sprite _square;

    private void Start()
    {
        nextSpawnY = 10f;
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
        float progress = Mathf.Clamp01(y / difficultyRampHeight);
        float fullWidth = playAreaHalfWidth * 2f;

        // Ledge covers between 40% and 70% of the width as difficulty ramps
        float minCoverage = Mathf.Lerp(0.4f, 0.55f, progress);
        float maxCoverage = Mathf.Lerp(0.55f, 0.75f, progress);
        float ledgeWidth = fullWidth * Random.Range(minCoverage, maxCoverage);

        // Alternate sides: left, right, left, right...
        bool fromLeft = !lastFromLeft;
        lastFromLeft = fromLeft;

        Color colour = GetBiomeColour(y);

        // Parent
        GameObject obstacle = new GameObject("Obstacle");
        obstacle.transform.position = new Vector3(0f, y, 0f);

        float cx;
        if (fromLeft)
        {
            cx = -playAreaHalfWidth + ledgeWidth / 2f;
        }
        else
        {
            cx = playAreaHalfWidth - ledgeWidth / 2f;
        }

        CreateBlock(obstacle.transform, cx, y, ledgeWidth, colour);

        // Score trigger on the open side
        float openWidth = fullWidth - ledgeWidth;
        float scoreCx = fromLeft
            ? playAreaHalfWidth - openWidth / 2f
            : -playAreaHalfWidth + openWidth / 2f;

        GameObject scoreTrigger = new GameObject("ScoreZone");
        scoreTrigger.tag = "ScoreZone";
        scoreTrigger.transform.SetParent(obstacle.transform);
        scoreTrigger.transform.position = new Vector3(scoreCx, y, 0f);
        BoxCollider2D scoreCol = scoreTrigger.AddComponent<BoxCollider2D>();
        scoreCol.size = new Vector2(openWidth * 0.5f, barThickness);
        scoreCol.isTrigger = true;
    }

    private void CreateBlock(Transform parent, float x, float y, float width, Color colour)
    {
        GameObject block = new GameObject("Block");
        block.transform.SetParent(parent);
        block.transform.position = new Vector3(x, y, 0f);
        block.transform.localScale = new Vector3(width, barThickness, 1f);
        block.tag = "Obstacle";

        SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquare();
        sr.color = colour;
        sr.sortingLayerName = "Obstacles";

        BoxCollider2D col = block.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

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

    private void Cleanup()
    {
        float destroyBelow = player.position.y - despawnBelow;
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
        nextSpawnY = 10f;
        lastFromLeft = false;
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