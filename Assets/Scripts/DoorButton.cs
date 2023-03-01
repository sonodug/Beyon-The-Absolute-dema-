using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] private HorizontalDoor _door;
    [SerializeField] private Transform _doorPoint;
    [SerializeField] private Transform _Totarget;
    [SerializeField] private float _doorOpenDuration;
    
    private bool _doorOpened;
    public bool DoorOpened => _doorOpened;

    public void OpenDoor()
    {
        _doorOpened = true;
        StartCoroutine(OpenDoorCoroutine());
    }
    
    private IEnumerator OpenDoorCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _doorOpenDuration)
        {
            timeElapsed += Time.deltaTime;
            _doorPoint.position = Vector3.Lerp(_doorPoint.position, _Totarget.position, timeElapsed / (_doorOpenDuration * 100.0f));
            yield return null;
        }
    }
}
