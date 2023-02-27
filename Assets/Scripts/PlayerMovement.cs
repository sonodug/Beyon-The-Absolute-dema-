using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravity;

    [Header("Controls")]
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;

    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canCrouch = true;
    
    private CharacterController _controller;
    
    private bool _isWalking;
    private bool _shouldJump => Input.GetKeyDown(JumpKey) && _controller.isGrounded;
    private bool _isSprinting => _canSprint && Input.GetKey(SprintKey);
    private bool _isCrouching => _canCrouch && Input.GetKey(CrouchKey);
    private bool _isFacingRight = true;

    private bool _canSprint = true;
    private bool _canMove = true;
    
    private Collider[] _colliders;

    private Vector3 _movementDirection;
    private Vector3 _movementInput;
    private float _currentSpeed;

    private void Start()
    {
        _colliders = new Collider[10];
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_canMove)
        {
            HandleMovementInput();
            
            TryToFlip();
            if (_canJump)
                HandleJump();
            
            if (_canCrouch)
                HandleCrouch();
            
            ApplyMovements();
        }
    }

    private void HandleMovementInput()
    {
        if (_isSprinting)
            _currentSpeed = _sprintSpeed;
        else if (_isCrouching)
            _currentSpeed = _crouchSpeed;
        else
            _currentSpeed = _walkSpeed;

        _movementInput = new Vector3(_currentSpeed * Input.GetAxisRaw("Horizontal"), 0.0f, 0.0f);
        float moveDirectionY = _movementDirection.y;
        _movementDirection = transform.TransformDirection(Vector3.right) * (_movementInput.x * (_isFacingRight ? 1 : -1));
        _movementDirection.y = moveDirectionY;
    }
    
    private void HandleJump()
    {
        if (_shouldJump)
            _movementDirection.y = _jumpForce;
    }

    private void HandleCrouch()
    {
        
    }

    private void TryToFlip()
    {
        if (_movementDirection.x < 0 && _isFacingRight)
        {
            transform.Rotate(0f, 180f, 0f);
            _isFacingRight = !_isFacingRight;
        }
        else if (_movementDirection.x > 0 && !_isFacingRight)
        {
            transform.Rotate(0f, -180f, 0f);
            _isFacingRight = !_isFacingRight;
        }
    }
    
    private void ApplyMovements()
    {
        if (!_controller.isGrounded)
            _movementDirection.y -= _gravity * Time.deltaTime;

        _controller.Move(_movementDirection * Time.deltaTime);
    }
}
