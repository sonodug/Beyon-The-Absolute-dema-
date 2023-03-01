using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode InteractiveKey = KeyCode.E;
    private bool _canInteractive = false;
    private bool _shouldOpenDoor => _canInteractive && Input.GetKeyDown(InteractiveKey);
    private bool _capsuleDestroyed = false;
    
    private Generator _currentActiveGenerator;
    private Button _currentActiveButton;
    private CallElevatorButton _currentActiveCallElevatorButton;

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
            if (_currentActiveButton != null)
            {
                if (!_currentActiveButton.DoorOpened)
                {
                    _currentActiveButton.OpenDoor();
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
    }

    public void Dead()
    {
        Destroy(gameObject);
    }

    public void HandleInteractiveWithGenerator(Generator generator, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveGenerator = generator;
    }
    
    public void HandleInteractiveWithDoorButton(Button button, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveButton = button;
    }
    
    public void HandleInteractiveWithElevatorTumbler(CallElevatorButton callElevatorButton, bool interactiveStatus)
    {
        _canInteractive = interactiveStatus;
        _currentActiveCallElevatorButton = callElevatorButton;
    }
}
