using UnityEngine;
using UnityEngine.InputSystem;

public class PLayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public float JumpForce = 10f;
    public float Speed = 10f;
    public bool GoingRight;

    public bool IsDead = false;
    public bool Frozen = true;
    public int Score = 0;
    public float HighestY = 0f;

    private Vector3 startPos;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        GoingRight = true;
    }

    private void Update()
    {
        if (IsDead || Frozen) return;

        if (transform.position.y > HighestY)
            HighestY = transform.position.y;
    }

    public void OnJump()
    {
        if (IsDead || Frozen) return;

        Debug.Log("Jump");
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, JumpForce);

        if (GoingRight == true)
        {
            _rigidbody.AddForce(transform.right * Speed);
        }
        else if (GoingRight == false)
        {
            _rigidbody.AddForce(transform.right * -Speed);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            GoingRight = !GoingRight;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fog"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.gravityScale = 0f;

        FindFirstObjectByType<GameOverController>().DisplayScore(Score);
    }

    public void Freeze()
    {
        Frozen = true;
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.gravityScale = 0f;
        }
    }

    public void Unfreeze(float gravityScale = 1f)
    {
        Frozen = false;
        if (_rigidbody != null)
        {
            _rigidbody.gravityScale = gravityScale;
        }
    }

    public void ResetPlayer(float gravityScale = 1f)
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.gravityScale = 0f;
        GoingRight = true;
        IsDead = false;
        Frozen = true;
        Score = 0;
        HighestY = 0f;
    }

    public void OnQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
