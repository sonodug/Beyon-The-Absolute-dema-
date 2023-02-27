using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal.Converters;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement params")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _sprintSpeed;
    
    [Header("Jumping params")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravity;

    [Header("Crouch params")]
    [SerializeField] private float _crouchHeight = 0.425f;
    [SerializeField] private float _timeToCrouch = 0.25f;
    [SerializeField] private Vector3 _crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 _standingCenter = new Vector3(0, 0, 0);

    [Header("Controls")]
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;

    [Header("Approve functional")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canSprint = true;
    [SerializeField] private bool _canCrouch = true;
    
    private CharacterController _controller;
    
    private bool _isWalking;
    private bool _shouldJump => Input.GetKeyDown(JumpKey) && _controller.isGrounded;
    private bool _shouldCrouch => Input.GetKeyDown(CrouchKey) && !_isCrouchAnimationActive && _controller.isGrounded;
    private bool _isSprinting => _canSprint && Input.GetKey(SprintKey);
    private bool _isCrouching;
    private bool _isCrouchAnimationActive;
    private float _standingHeight;
    
    private bool _isFacingRight = true;

    private Collider[] _colliders;

    private Vector3 _movementDirection;
    private Vector3 _movementInput;
    private float _currentSpeed;

    private void Start()
    {
        _colliders = new Collider[10];
        _controller = GetComponent<CharacterController>();
        _standingHeight = _controller.height;
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
        if (_shouldCrouch)
            StartCoroutine(CrouchStand());
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

    private IEnumerator CrouchStand()
    {
        _isCrouchAnimationActive = true;
        float timeElapsed = 0;
        float targetHeight = _isCrouching ? _standingHeight : _crouchHeight;
        float currentHeight = _controller.height;
        Vector3 targetCenter = _isCrouching ? _standingCenter : _crouchingCenter;
        Vector3 currentCenter = _controller.center;

        while (timeElapsed < _timeToCrouch)
        {
            _controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
            _controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _controller.height = targetHeight;
        _controller.center = targetCenter;
        _isCrouching = !_isCrouching;
        
        _isCrouchAnimationActive = false;
    }
}
