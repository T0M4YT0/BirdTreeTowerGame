using UnityEngine;

public class FogWall : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float baseSpeed = 1.5f;
    [SerializeField] private float acceleration = 0.008f;
    [SerializeField] private float startOffset = 18f;
    [SerializeField] private Color fogColour = new Color(0.12f, 0.1f, 0.08f, 0.95f);

    private float currentSpeed;
    private bool active;
    private SpriteRenderer sr;

    private void Start()
    {
        // Build visual
        GameObject visual = new GameObject("FogVisual");
        visual.transform.SetParent(transform);
        visual.transform.localPosition = new Vector3(0f, -15f, 0f);
        visual.transform.localScale = new Vector3(12f, 30f, 1f);

        sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = ObstacleSpawner.GetSquare();
        sr.color = fogColour;
        sr.sortingLayerName = "Foreground";

        // Trigger on top edge
        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(12f, 0.3f);
        col.isTrigger = true;
        gameObject.tag = "Fog";

        ResetFog();
    }

    private void Update()
    {
        if (!active) return;

        currentSpeed += acceleration * Time.deltaTime;
        Vector3 pos = transform.position;
        pos.y += currentSpeed * Time.deltaTime;
        transform.position = pos;
    }

    public void Activate() => active = true;

    public void ResetFog()
    {
        active = false;
        currentSpeed = baseSpeed;
        float startY = player != null ? player.position.y - startOffset : -startOffset;
        transform.position = new Vector3(0f, startY, 0f);
    }
}
