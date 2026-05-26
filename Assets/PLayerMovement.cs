using UnityEngine;
using UnityEngine.InputSystem;

public class PLayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    public float JumpForce = 10f;


    void OnJump()
    {
        Debug.Log("Jump");
        //_rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, JumpForce);
    }
}
