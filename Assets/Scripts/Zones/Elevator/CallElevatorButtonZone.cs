using UnityEngine;

public class CallElevatorButtonZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithElevatorTumbler(gameObject.GetComponentInParent<CallElevatorButton>(), true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithElevatorTumbler(null, false);
        }
    }
}
