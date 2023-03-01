using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractiveZone : MonoBehaviour
{
    public UnityAction OpenedText;
    public UnityAction ClosedText;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithDoorButton(gameObject.GetComponentInParent<DoorButton>(), true);
            OpenedText?.Invoke();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithDoorButton(null, false);
            ClosedText?.Invoke();
        }
    }
}
