using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lamp : MonoBehaviour
{
    [SerializeField] private Light _spotLight;
    [SerializeField] private BoxCollider _deathArea;

    public void Disable()
    {
        _spotLight.enabled = false;
        _deathArea.enabled = false;
    }
    
    public void Enable()
    {
        _spotLight.enabled = true;
        _deathArea.enabled = true;
    }
}
