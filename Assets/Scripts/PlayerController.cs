using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Physics (the 'flap' axis)")]
    [SerializeField] private float horizontalGravity = 12f;   // pulls bird to one side
    [SerializeField] private float flapImpulse = 6f;          // pushes bird the other way
    [SerializeField] private float maxHorizontalSpeed = 10f;

    [Header("Vertical Movement (auto-rise)")]
    [SerializeField] private float riseSpeed = 3f;            // constant upward speed
    [SerializeField] private float riseAcceleration = 0.02f;  // gets faster over time

    [Header("Play Area")]
    [SerializeField] private float playAreaHalfWidth = 1.9f;

    [Header("Visuals")]
    [SerializeField] private float tiltSmoothing = 8f;
    [SerializeField] private float maxTiltAngle = 40f;

    private float velocityX;
    private float currentRiseSpeed;
    private int gravityDirection = 1; // 1 = falling right, -1 = falling left
    private bool isDead = false;
    [SerializeField] private bool frozen = true;
    private float highestY = 0f;

    public float VelocityX => velocityX;
    public bool IsDead => isDead;
    public float HighestY => highestY;
    public int Score { get; set; }

    public void Freeze() => frozen = true;
    public void Unfreeze() => frozen = false;

    private void Update()
    {
        if (isDead || frozen) return;

        // Flap input - reverses horizontal direction
        if (FlapPressed())
            Flap();

        // Horizontal gravity pulls bird sideways
        velocityX += horizontalGravity * gravityDirection * Time.deltaTime;
        velocityX = Mathf.Clamp(velocityX, -maxHorizontalSpeed, maxHorizontalSpeed);

        // Auto-rise
        currentRiseSpeed += riseAcceleration * Time.deltaTime;

        // Move
        Vector3 pos = transform.position;
        pos.x += velocityX * Time.deltaTime;
        pos.y += currentRiseSpeed * Time.deltaTime;

        // Bounce off walls
        if (pos.x > playAreaHalfWidth)
        {
            pos.x = playAreaHalfWidth;
            velocityX = -Mathf.Abs(velocityX) * 0.3f;
            gravityDirection = -1;
        }
        else if (pos.x < -playAreaHalfWidth)
        {
            pos.x = -playAreaHalfWidth;
            velocityX = Mathf.Abs(velocityX) * 0.3f;
            gravityDirection = 1;
        }

        transform.position = pos;

        if (pos.y > highestY)
            highestY = pos.y;

        // Tilt based on horizontal velocity
        float targetAngle = -Mathf.Clamp(velocityX / maxHorizontalSpeed, -1f, 1f) * maxTiltAngle;
        float current = transform.eulerAngles.z;
        if (current > 180f) current -= 360f;
        float smooth = Mathf.Lerp(current, targetAngle, tiltSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, smooth);
    }

    public static bool FlapPressed()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) return true;
            if (Keyboard.current.anyKey.wasPressedThisFrame) return true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        var gp = Gamepad.current;
        if (gp != null)
        {
            if (gp.buttonSouth.wasPressedThisFrame) return true;
            if (gp.buttonWest.wasPressedThisFrame) return true;
            if (gp.buttonEast.wasPressedThisFrame) return true;
            if (gp.buttonNorth.wasPressedThisFrame) return true;
            if (gp.rightTrigger.wasPressedThisFrame) return true;
            if (gp.leftTrigger.wasPressedThisFrame) return true;
        }

        return false;
    }

    public void Flap()
    {
        if (isDead) return;
        // Reverse gravity direction and give impulse
        gravityDirection *= -1;
        velocityX = flapImpulse * gravityDirection;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        velocityX = 0f;
        currentRiseSpeed = 0f;
    }

    public void ResetBird(Vector3 startPos)
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        velocityX = 0f;
        currentRiseSpeed = riseSpeed;
        gravityDirection = 1;
        isDead = false;
        frozen = true;
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
