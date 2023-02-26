using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundedRadius;
    
    private bool _isGrounded;
    private Collider[] _colliders;

    private void Start()
    {
        _colliders = new Collider[10];
    }

    private void FixedUpdate()
    {
        _isGrounded = false;
        
        int collisionNumbers = Physics.OverlapSphereNonAlloc(_groundCheck.position, _groundedRadius, _colliders, _groundMask);
        
        if (collisionNumbers > 0)
        {
            _isGrounded = true;
        }
    }
    
    public void Move(Vector3 movement, float speed)
    {
        transform.parent.Translate(movement * (speed * Time.fixedDeltaTime));
    }
    
    public void Jump(Rigidbody rb, float jumpForce)
    {
        if (Input.GetAxis("Jump") > 0)
        {
            if (_isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce);
            }
        }
    }
}
