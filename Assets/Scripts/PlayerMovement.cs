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
    
    [SerializeField] private float _ladderSpeed;
    
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
    [SerializeField] private KeyCode ClimbKey = KeyCode.W;
    [SerializeField] private KeyCode DownLadderKey = KeyCode.S;
    [SerializeField] private KeyCode MiddleLadderKey = KeyCode.E;

    [Header("Approve functional")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canSprint = true;
    [SerializeField] private bool _canCrouch = true;
    [SerializeField] private bool _canSlideOnWall = true;
    [SerializeField] private bool _canWallJump = true;
    [SerializeField] private bool _canClimbOnLedge = true;
    [SerializeField] private bool _canClimbOnSideLadder = true;
    [SerializeField] private bool _canClimbOnMiddleLadder = true;

    [Header("Head point")]
    [SerializeField] private Transform _ceil;
    [SerializeField] private Transform _wallCheck;

    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private LayerMask _ledgeMask;
    [SerializeField] private LayerMask _sideLadderMask;
    [SerializeField] private LayerMask _middleLadderMask;
    [SerializeField] private float _slideSpeed;
    [SerializeField] private float _blockInputDuration;
    [SerializeField] private float _climbAnimationDuration;

    [SerializeField] private float _wallRayDistance;
    [SerializeField] private float _middleLadderRayDistance;
    [SerializeField] private float _wallLedgeDistance;
    [SerializeField] private float _ledgeRayIntervalY = 0.2f;
    
    [Header("Climb finish offsets")]
    [SerializeField] private float _climbFinishOffsetX;
    [SerializeField] private float _climbFinishOffsetY;

    private CharacterController _controller;
    
    private bool _isWalking;
    private bool _shouldJump => Input.GetKeyDown(JumpKey) && _controller.isGrounded && !_isOnSideLadder;
    private bool _shouldWallJump => Input.GetKeyDown(JumpKey) && (_isOnWall || _isOnSideLadder) && !_controller.isGrounded;
    private bool _shouldCrouch => Input.GetKeyDown(CrouchKey) && !_isCrouchAnimationActive && _controller.isGrounded;
    private bool _shouldClimbLedge => Input.GetKeyDown(ClimbKey);
    private bool _shouldClimbSideLadder => Input.GetKey(ClimbKey) && !Input.GetKey(DownLadderKey) && _isOnSideLadder;
    private bool _shouldDownSideLadder => Input.GetKey(DownLadderKey) && !Input.GetKey(ClimbKey) && _isOnSideLadder;
    private bool _isInputOnSideLadderActive => !Input.GetKey(DownLadderKey) && !Input.GetKey(ClimbKey) && _isOnSideLadder;
    private bool _shouldMiddleLadderUsed => Input.GetKeyDown(MiddleLadderKey) && _isOnMiddleLadder && (_controller.isGrounded || _isMiddleLadderKeyPressed);
    private bool _shouldClimbMiddleLadder => Input.GetKey(ClimbKey) && !Input.GetKey(DownLadderKey) && _isOnMiddleLadder;
    private bool _shouldDownMiddleLadder => Input.GetKey(DownLadderKey) && !Input.GetKey(ClimbKey) && _isOnMiddleLadder;
    private bool _isInputOnMiddleLadderActive => !Input.GetKey(DownLadderKey) && !Input.GetKey(ClimbKey) && _isOnMiddleLadder;
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
    private bool _isOnSideLadder = false;
    private bool _isOnMiddleLadder = false;
    private bool _moveDirectionYUpdated = true;
    private bool _blockInputOnSideLadderMovement = false;
    private bool _blockInputOnMiddleLadderMovement = false;
    private bool _isReadyToMiddleLadderMovement = false;
    private bool _blockInputOnWallMovement => _isOnWall && !_controller.isGrounded;
    private bool _blockInputMovementAfterWallJump = false;
    private bool _blockInputLedge = false;
    private bool _blockWallJump = false;
    private float _middleOffsetY;
    
    private float _delay = 0.15f;

    private bool _setGravity = false;

    private bool _isMiddleLadderKeyPressed = false;
    private bool _canJumpFromMiddleLadder = false;
    private Vector3 _MiddleLadderCheckVector = Vector3.forward;
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _standingHeight = _controller.height;
    }

    private void Update()
    {
        if (_canMove)
        {
            if (!_blockInputOnWallMovement && !_blockInputMovementAfterWallJump && !_blockInputOnSideLadderMovement && !_blockInputOnMiddleLadderMovement)
                HandleMovementInput();
            
            TryToFlip();

            if ((_canJump && !_blockInputOnMiddleLadderMovement) || _canJumpFromMiddleLadder)
            {
                HandleJump();
            }

            if (_canCrouch && !_blockInputOnMiddleLadderMovement)
                HandleCrouch();
            
            if (_canWallJump)
                HandleWallJump();
            
            if (_canClimbOnLedge)
                HandleCheckOnLedge();

            if (_canClimbOnSideLadder)
            {
                HandleCheckOnSideLadder();
                HandleOnSideLadderMovement();
                HandleCheckOnLedgeSideLadder();
            }

            if (_canClimbOnMiddleLadder)
            {
                HandleCheckOnMiddleLadder();
                HandleOnMiddleLadderMovement();

                if (_isReadyToMiddleLadderMovement)
                {
                    ApplyMiddleLadderMovement();
                }

                HandleCheckOnLedgeMiddleLadder();
            }

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
        if (_shouldJump || Input.GetKeyDown(JumpKey) && _canJumpFromMiddleLadder)
        {
            if (_canJumpFromMiddleLadder)
            {
                EscapeFromLadder();
            }
            
            if (!Physics.Raycast(_ceil.position, Vector3.up, 1.0f))
            {
                _movementDirection.y = _jumpForce;
            }
        }
    }

    private void HandleWallJump()
    {
        if (_shouldWallJump && !_isSprinting && !_blockWallJump)
        {
            StartCoroutine(BlockInputTimerCoroutine());
            _movementDirection.y = _wallJumpForce;
            _movementDirection.x = _wallJumpXForce * (_isFacingRight ? -1 : 1);
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
    
    private void HandleCheckOnSideLadder()
    {
        _isOnSideLadder = Physics.Raycast(_wallCheck.position, Vector3.right * (_isFacingRight ? 1 : -1), _wallRayDistance, _sideLadderMask);
    }

    private void HandleCheckOnMiddleLadder()
    {
        _isOnMiddleLadder = Physics.Raycast(_wallCheck.position, Vector3.forward, _middleLadderRayDistance, _middleLadderMask);
    }

    private void HandleOnMiddleLadderMovement()
    {
        if (_isOnMiddleLadder)
        {
            if (_shouldMiddleLadderUsed)
            {
                if (!_isMiddleLadderKeyPressed)
                {
                    if (_isCrouching)
                    {
                        _controller.enabled = false;
                        transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2 - 0.1f, 0.0f);
                        _controller.enabled = true;
                        float prevTimeToCrouch = _timeToCrouch;
                        _timeToCrouch = 0;
                        StartCoroutine(CrouchStandCoroutine());
                        _timeToCrouch = prevTimeToCrouch;
                    }
                    transform.Rotate(0f, (_isFacingRight ? -90.0f : 90.0f), 0f);
                    
                    _isMiddleLadderKeyPressed = true;
                    _blockInputOnMiddleLadderMovement = true;
                    _movementDirection.x = 0.0f;
                    _movementDirection.y = 0.0f;
                    _isReadyToMiddleLadderMovement = !_isReadyToMiddleLadderMovement;
                    _MiddleLadderCheckVector = Vector3.right;
                }
                else if(_isMiddleLadderKeyPressed)
                {
                    EscapeFromLadder();
                }
            }
        }
    }

    private void EscapeFromLadder()
    {
        transform.Rotate(0f, (_isFacingRight ? 90.0f : -90.0f), 0f);
        _isMiddleLadderKeyPressed = false;
        _blockInputOnMiddleLadderMovement = false;
        _isReadyToMiddleLadderMovement = !_isReadyToMiddleLadderMovement;
        _MiddleLadderCheckVector = Vector3.forward;
    }

    private void ApplyMiddleLadderMovement()
    {
        if (_shouldClimbMiddleLadder)
        {
            _movementDirection.y = _ladderSpeed;
        }

        if (_shouldDownMiddleLadder)
        {
            _movementDirection.y = -_ladderSpeed;
        }

        if (_isInputOnMiddleLadderActive)
        {
            _movementDirection.y = 0.0f;
        }
    }
    
    private void HandleOnSideLadderMovement()
    {
        if (_isOnSideLadder)
        {
            _blockInputOnSideLadderMovement = true;
            if (_setGravity == false)
            {
                _movementDirection.y = 0.0f;
                _setGravity = true;
                if (_isCrouching)
                    StartCoroutine(CrouchStandCoroutine());
            }
            
            if (_shouldClimbSideLadder)
            {
                _movementDirection.y = _ladderSpeed;
            }

            if (_shouldDownSideLadder)
            {
                _movementDirection.y = -_ladderSpeed;
            }

            if (_isInputOnSideLadderActive)
            {
                _movementDirection.y = 0.0f;
            }
        }
        else
        {
            _setGravity = false;
            _blockInputOnSideLadderMovement = false;
        }
    }

    private void HandleCheckOnLedge()
    {
        if (_isOnWall)
        {
            _isOnLedge = !Physics.Raycast
            (
                new Vector3(_wallCheck.position.x, _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
                Vector3.right * (_isFacingRight ? 1 : -1),
                _wallLedgeDistance,
                _wallMask
            );
        }
        else
            _isOnLedge = false;

        if (_isOnLedge && _movementDirection.y < 0)
        {
            _movementDirection.y = 0.0f;
            CalculateMiddleInterval();
            HandleClimbOnLedge();
        }
    }
    
    private void HandleCheckOnLedgeSideLadder()
    {
        if (_isOnSideLadder)
        {
            _isOnLedge = !Physics.Raycast
            (
                new Vector3(_wallCheck.position.x, _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
                Vector3.right * (_isFacingRight ? 1 : -1),
                _wallLedgeDistance,
                _sideLadderMask
            );
        }
        else
            _isOnLedge = false;

        if (_isOnLedge && _movementDirection.y > 0)
        {
            _movementDirection.y = 0.0f;
            CalculateMiddleInterval();
            HandleClimbOnLedgeLadder();
        }
    }
    
    private void HandleCheckOnLedgeMiddleLadder()
    {
        if (_isOnMiddleLadder)
        {
            _isOnLedge = !Physics.Raycast
            (
                new Vector3(_wallCheck.position.x, _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
                Vector3.forward,
                _wallLedgeDistance,
                _middleLadderMask
            );
        }
        else
        {
            _isOnLedge = false;
        }

        if (!_isOnLedge)
            _canJumpFromMiddleLadder = false;
        
        if (_isOnLedge && _movementDirection.y >= 0)
        {
            _movementDirection.y = 0.0f;
            _canJumpFromMiddleLadder = true;
            CalculateMiddleInterval();
        }
    }

    private void CalculateMiddleInterval()
    {
        RaycastHit hit;

        Ray ray = new Ray
        (
            new Vector3(_wallCheck.position.x + _wallRayDistance * (_isFacingRight ? 1 : -1), _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
            Vector3.down
        );
        
        Physics.Raycast
        (
            ray,
            out hit,
            _wallRayDistance,
            _ledgeMask
        );

        _middleOffsetY = hit.distance;
        
        _controller.enabled = false;
        transform.position = new Vector3(transform.position.x, transform.position.y - _middleOffsetY, 0);
        _controller.enabled = true;
    }

    private void HandleClimbOnLedge()
    {
        if (_shouldClimbLedge && !_blockInputLedge)
        {
            //Animation
            StartCoroutine(SimulateClimbAnimationCoroutine());
        }
    }
    
    private void HandleClimbOnLedgeLadder()
    {
        if (!_blockInputLedge)
        {
            //Animation
            StartCoroutine(SimulateClimbAnimationCoroutine());
        }
    }

    private void HandleClimbOnMiddleLadder()
    {
        if (!_blockInputLedge)
        {
            //Animation
            StartCoroutine(SimulateClimbAnimationCoroutine());
        }
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
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine
        (
            new Vector3(_wallCheck.position.x + _wallRayDistance * (_isFacingRight ? 1 : -1), _wallCheck.position.y + _ledgeRayIntervalY, 0.0f),
            new Vector3(_wallCheck.position.x + _wallRayDistance * (_isFacingRight ? 1 : -1), _wallCheck.position.y, 0.0f)
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
        else if (!_controller.isGrounded && !_blockInputOnSideLadderMovement && !_blockInputOnMiddleLadderMovement)
        {
            _movementDirection.y -= _gravity * Time.deltaTime;
        }
        else
        {
            _moveDirectionYUpdated = true;
        }

        _movementDirection.z = 0;
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

    private IEnumerator SimulateClimbAnimationCoroutine()
    {
        _blockInputLedge = true;
        _blockWallJump = true;
        float timeElapsed = 0;

        while (timeElapsed < _climbAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        _controller.enabled = false;
        transform.position = new Vector3(transform.position.x + _climbFinishOffsetX * (_isFacingRight ? 1 : -1),
            transform.position.y + transform.localScale.y - _climbFinishOffsetY, 
            0.0f);
        _controller.enabled = true;
        
        _blockInputLedge = false;
        _blockWallJump = false;
    }
    
    private IEnumerator DelayCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _delay)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
