using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform _targetFrom;
    [SerializeField] private Transform _targetTo;
    [SerializeField] private float _moveToTargetDuration;
    [SerializeField] private float _delayOnComingDuration;
    [SerializeField] private Transform _currentTargetPoint;
    
    private bool _applyMovement = false;
    private float _prevElapsedTime = 0;
    
    private bool _isOnTargetTo = false;
    private bool _isOnTargetToCallButton = false;
    
    public event UnityAction ElevatorComing;

    private void Update()
    {
        if (_applyMovement || _isOnTargetToCallButton)
        {
            _prevElapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _currentTargetPoint.position, _prevElapsedTime / (_moveToTargetDuration * 100.0f));

            if (transform.position == _currentTargetPoint.position && !_isOnTargetTo && !_isOnTargetToCallButton)
                StartCoroutine(DelayCoroutine());
            else if (transform.position == _currentTargetPoint.position && _isOnTargetTo && !_isOnTargetToCallButton)
                StartCoroutine(DelayCoroutine());
            else if (transform.position == _currentTargetPoint.position && _isOnTargetToCallButton)
            {
                ElevatorComing?.Invoke();
                _isOnTargetToCallButton = false;
            }
        }
    }
    
    private IEnumerator DelayCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _delayOnComingDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        if (_isOnTargetTo)
            _currentTargetPoint = _targetTo;
        else
            _currentTargetPoint = _targetFrom;

        _isOnTargetTo = !_isOnTargetTo;

        _prevElapsedTime = 0;
    }

    public void ApplySignalFromButton(Transform target)
    {
        _currentTargetPoint = target;
        _isOnTargetToCallButton = true;
    }

    public void EnableElevator()
    {
        _applyMovement = true;
    }
    
    public void StopElevator()
    {
        _applyMovement = false;
    }
}
