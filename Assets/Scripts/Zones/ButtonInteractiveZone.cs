using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractiveZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithDoorButton(gameObject.GetComponentInParent<DoorButton>(), true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithDoorButton(null, false);
        }
    }
}
