using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode InteractiveKey = KeyCode.E;
    [SerializeField] private KeyCode StunKey = KeyCode.Z;
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private float _enemyDetectedRayDistance;
    [SerializeField] private Transform _spawnPoint;
    
    private bool _canInteractive = false;
    private bool _shouldOpenDoor => _canInteractive && Input.GetKeyDown(InteractiveKey);
    private bool _shouldStunEnemy => !_isEnemyStunned && _isEnemyNear && Input.GetKeyDown(StunKey);
    private bool _capsuleDestroyed = false;
    
    private Generator _currentActiveGenerator;
    private DoorButton _currentActiveDoorButton;
    private CallElevatorButton _currentActiveCallElevatorButton;

    public UnityAction Stunned;

    private PlayerMovement _movement;
    private bool _isEnemyNear;
    private bool _isEnemyStunned;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (_shouldOpenDoor && !_capsuleDestroyed)
        {
            if (_currentActiveGenerator != null)
            {
                if (_currentActiveGenerator.DoorOpened)
                {
                    _currentActiveGenerator.DestroyCapsule();
                    _capsuleDestroyed = true;
                }
            
                _currentActiveGenerator.OpenDoor();
            }
        }

        if (_shouldOpenDoor)
        {
            if (_currentActiveDoorButton != null)
            {
                if (!_currentActiveDoorButton.DoorOpened)
                {
                    _currentActiveDoorButton.OpenDoor();
                }
            }
        }
        
        if (_shouldOpenDoor)
        {
            if (_currentActiveCallElevatorButton != null)
            {
                if (!_currentActiveCallElevatorButton.IsElevatorCalled)
                {
                    _currentActiveCallElevatorButton.CallElevator();
                }
            }
        }

        HandleNearEnemy();

        if (_shouldStunEnemy)
        {
            Stunned?.Invoke();
        }
    }

    private void HandleNearEnemy()
    {
        _isEnemyNear = Physics.Raycast(_movement.WallCheck.position, Vector3.right * (_movement.IsFacingRight ? 1 : -1), _enemyDetectedRayDistance, _enemyMask);
        RaycastHit hit;

        Ray ray = new Ray
        (
            _movement.WallCheck.position,
            Vector3.right * (_movement.IsFacingRight ? 1 : -1)
        );

        if (Physics.Raycast(ray, out hit, _enemyDetectedRayDistance, _enemyMask))
        {
            _isEnemyStunned = hit.collider.GetComponent<Enemy>().IsStunned;
        }
    }

    public void Dead()
    {
        _movement.MoveToSpawnPoint(_spawnPoint);
    }

    public void HandleInteractiveWithGenerator(Generator generator, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveGenerator = generator;
    }
    
    public void HandleInteractiveWithDoorButton(DoorButton doorButton, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveDoorButton = doorButton;
    }
    
    public void HandleInteractiveWithElevatorTumbler(CallElevatorButton callElevatorButton, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveCallElevatorButton = callElevatorButton;
    }
}
