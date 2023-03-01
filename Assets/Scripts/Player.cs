using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode InteractiveKey = KeyCode.E;
    private bool _canInteractive = false;
    private bool _shouldOpenDoor => _canInteractive && Input.GetKeyDown(InteractiveKey);
    private Generator _currentActiveGenerator;
    private bool _capsuleDestroyed = false;
    

    private void Update()
    {
        if (_shouldOpenDoor && !_capsuleDestroyed)
        {
            if (_currentActiveGenerator.DoorOpened)
            {
                _currentActiveGenerator.DestroyCapsule();
                _capsuleDestroyed = true;
            }
            
            _currentActiveGenerator.OpenDoor();
        }
    }

    public void Dead()
    {
        Destroy(gameObject);
    }

    public void HandleInteractive(Generator generator, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveGenerator = generator;
    }
}
