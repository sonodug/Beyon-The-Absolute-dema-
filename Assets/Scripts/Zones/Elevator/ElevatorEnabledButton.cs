using UnityEngine;

public class ElevatorEnabledButton : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            GetComponentInParent<Elevator>().EnableElevator();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            GetComponentInParent<Elevator>().StopElevator();
        }
    }
}
