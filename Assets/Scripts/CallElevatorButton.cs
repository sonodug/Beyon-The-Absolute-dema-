using UnityEngine;

public class CallElevatorButton : MonoBehaviour
{
    [SerializeField] private Elevator _elevator;
    [SerializeField] private Transform _targetTo;
    
    private bool _isCalled;
    public bool IsElevatorCalled => _isCalled;

    private void OnEnable()
    {
        _elevator.ElevatorComing += OnElevatorComing;
    }

    private void OnDisable()
    {
        _elevator.ElevatorComing -= OnElevatorComing;
    }

    public void CallElevator()
    {
        _elevator.ApplySignalFromButton(_targetTo);
        _isCalled = true;
    }

    public void OnElevatorComing()
    {
        _isCalled = false;
    }
}
