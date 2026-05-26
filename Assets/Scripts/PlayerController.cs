using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float flapImpulse = 8f;
    [SerializeField] private float maxFallSpeed = -14f;
    [SerializeField] private float maxRiseSpeed = 12f;

    [Header("Visuals")]
    [SerializeField] private float tiltSmoothing = 8f;
    [SerializeField] private float maxTiltAngle = 35f;

    private float velocityY;
    private bool isDead = false;
    private float highestY = 0f;

    public float VelocityY => velocityY;
    public bool IsDead => isDead;
    public float HighestY => highestY;
    public int Score { get; set; }

    private void Update()
    {
        if (isDead) return;

        // Flap
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)
            || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Flap();
        }

        // Gravity
        velocityY -= gravity * Time.deltaTime;
        velocityY = Mathf.Clamp(velocityY, maxFallSpeed, maxRiseSpeed);

        // Move
        Vector3 pos = transform.position;
        pos.y += velocityY * Time.deltaTime;
        transform.position = pos;

        if (pos.y > highestY)
            highestY = pos.y;

        // Tilt sprite based on velocity
        float targetAngle = Mathf.Clamp(velocityY / maxRiseSpeed, -1f, 1f) * maxTiltAngle;
        float current = transform.eulerAngles.z;
        if (current > 180f) current -= 360f;
        float smooth = Mathf.Lerp(current, targetAngle, tiltSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, smooth);
    }

    public void Flap()
    {
        if (isDead) return;
        velocityY = flapImpulse;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        velocityY = 0f;
    }

    public void ResetBird(Vector3 startPos)
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        velocityY = 0f;
        isDead = false;
        highestY = 0f;
        Score = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fog"))
            Die();
        if (other.CompareTag("ScoreZone"))
        {
            Score++;
            Destroy(other.gameObject);
        }
    }
}
