using UnityEngine;
using UnityEngine.Events;

public class CallElevatorButtonZone : MonoBehaviour
{
    public UnityAction OpenedText;
    public UnityAction ClosedText;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithElevatorTumbler(gameObject.GetComponentInParent<CallElevatorButton>(), true);
            OpenedText?.Invoke();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithElevatorTumbler(null, false);
            ClosedText?.Invoke();
        }
    }
}
