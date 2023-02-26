using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private PlayerMovementController _controller;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _controller.Jump(_rigidbody, _jumpForce);
        _controller.Move(
            FormMovementVector(),
            _speed);
    }

    private Vector3 FormMovementVector()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        return new Vector3(moveHorizontal, 0.0f, 0.0f);
    }
}
