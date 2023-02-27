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
    [SerializeField] private float _wallJumpForce;
    [SerializeField] private float _wallJumpXForce;
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
    [SerializeField] private bool _canSlideOnWall = true;
    [SerializeField] private bool _canWallJump = true;
    [SerializeField] private bool _canClimbOnLedge = true;

    [Header("Head point")]
    [SerializeField] private Transform _ceil;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private Transform _wallCheckUp;
    
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private float _wallCheckRadius;
    [SerializeField] private float _slideSpeed;
    [SerializeField] private float _blockInputDuration;

    [SerializeField] private float _climbingWallAnimDuration;
    [SerializeField] private float _wallRayDistance;
    [SerializeField] private float _ledgeRayIntervalY = 0.2f;
    
    private CharacterController _controller;
    
    private bool _isWalking;
    private bool _shouldJump => Input.GetKeyDown(JumpKey) && _controller.isGrounded;
    private bool _shouldWallJump => Input.GetKeyDown(JumpKey) && _isOnWall && !_controller.isGrounded;
    private bool _shouldCrouch => Input.GetKeyDown(CrouchKey) && !_isCrouchAnimationActive && _controller.isGrounded;
    private bool _isSprinting => _canSprint && Input.GetKey(SprintKey);
    private bool _isCrouching;
    private bool _isCrouchAnimationActive;
    private float _standingHeight;
    
    private bool _isFacingRight = true;

    private Vector3 _movementDirection;
    private Vector3 _movementInput;
    private float _currentSpeed;
    
    private bool _isOnWall = false;
    private bool _isOnLedge = false;
    private bool _isOnWallUp = false;
    private bool _moveDirectionYUpdated = true;
    private bool _blockInputMovement => _isOnWall && !_controller.isGrounded;
    private bool _blockInputMovementAfterWallJump = false;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _standingHeight = _controller.height;
    }

    private void Update()
    {
        if (_canMove)
        {
            if (!_blockInputMovement && !_blockInputMovementAfterWallJump)
                HandleMovementInput();
            
            TryToFlip();
            
            if (_canJump)
                HandleJump();
            
            if (_canCrouch)
                HandleCrouch();
            
            if (_canWallJump)
                HandleWallJump();
            
            if (_canClimbOnLedge)
                HandleCheckOnLedge();

            ApplyMovements();
        }
    }

    private void HandleMovementInput()
    {
        if (_isSprinting && !_isCrouching)
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
        {
            if (!Physics.Raycast(_ceil.position, Vector3.up, 1.0f))
            {
                _movementDirection.y = _jumpForce;
            }
        }
    }

    private void HandleWallJump()
    {
        if (_shouldWallJump && !_isSprinting)
        {
            StartCoroutine(BlockInputTimerCoroutine());
            _movementDirection.x = _wallJumpXForce * (_isFacingRight ? -1 : 1);
            _movementDirection.y = _wallJumpForce;
        }
    }

    private void HandleCrouch()
    {
        if (_shouldCrouch)
            StartCoroutine(CrouchStandCoroutine());
    }
    
    private bool HandleCheckOnWall()
    {
        _isOnWall = Physics.Raycast(_wallCheck.position, Vector3.right * (_isFacingRight ? 1 : -1), _wallRayDistance, _wallMask) && !_controller.isGrounded;
        return _isOnWall;
    }

    private void HandleCheckOnLedge()
    {
        if (_isOnWall)
        {
            _isOnLedge = !Physics.Raycast
            (
                new Vector3(_wallCheck.position.x, _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
                Vector3.right * (_isFacingRight ? 1 : -1),
                _wallRayDistance,
                _wallMask
            );
        }
        else
            _isOnLedge = false;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheck.position,
            new Vector3(_wallCheck.position.x + _wallRayDistance * (_isFacingRight ? 1 : -1), _wallCheck.position.y, 0.0f));
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine
        (
            new Vector3(_wallCheck.position.x, _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
            new Vector3(_wallCheck.position.x + _wallRayDistance * (_isFacingRight ? 1 : -1),
                _wallCheck.position.y + _ledgeRayIntervalY, 0.0f)
        );
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
        if (HandleCheckOnWall() && _canSlideOnWall)
        {
            if (_moveDirectionYUpdated)
                _movementDirection.y = 0.1f;
            
            _movementDirection.y -= _slideSpeed * Time.deltaTime;
            _moveDirectionYUpdated = false;
        }
        else if (!_controller.isGrounded)
        {
            _movementDirection.y -= _gravity * Time.deltaTime;
        }
        else
        {
            _moveDirectionYUpdated = true;
        }

        _controller.Move(_movementDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStandCoroutine()
    {
        if (_isCrouching && Physics.Raycast(_ceil.position, Vector3.up, 0.3f))
            yield break;

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

    private IEnumerator BlockInputTimerCoroutine()
    {
        _blockInputMovementAfterWallJump = true;
        
        float timeElapsed = 0;

        while (timeElapsed < _blockInputDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _blockInputMovementAfterWallJump = false;
    }
}
