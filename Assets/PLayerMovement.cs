using UnityEngine;
using UnityEngine.InputSystem;

public class PLayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    public float JumpForce = 10f;
    public float Speed = 10f;

    public bool GoingRight;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        GoingRight = true;
    }
    void OnJump()
    {
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
        if (collision.gameObject.tag == "Wall")
        {
            if (GoingRight == true)
            {
                GoingRight = false;
            }
            else if (GoingRight == false)
            {
                GoingRight = true;
            }
        }
    }
}
