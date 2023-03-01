using System;
using System.Collections;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;
    [SerializeField] private Transform _doorPivot;
    [SerializeField] private float _doorOpenSpeed;
    [SerializeField] private float _doorOpenDuration;
    [SerializeField] private LightCapsule lightCapsule;

    private bool _isAnimationActive = false;
    
    private bool _doorOpened;
    public bool DoorOpened => _doorOpened;

    private void Update()
    {
        if (_isAnimationActive)
        {
            _doorPivot.rotation = Quaternion.Lerp(_doorPivot.rotation, Quaternion.Euler(_doorPivot.rotation.x, 120.0f, _doorPivot.rotation.z), Time.deltaTime * _doorOpenSpeed);
        }
    }

    public void OpenDoor()
    {
        _isAnimationActive = true;
        StartCoroutine(ExceptOpenDoorCoroutine());
    }

    public void DestroyCapsule()
    {
        Destroy(lightCapsule.gameObject);
        _lamp.DisableArea();
    }
    
    private IEnumerator ExceptOpenDoorCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _doorOpenDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _doorOpened = true;
    }
}
