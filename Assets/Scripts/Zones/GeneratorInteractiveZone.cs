using UnityEngine;

public class GeneratorInteractiveZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractive(gameObject.GetComponentInParent<Generator>(), true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractive(null, false);
        }
    }
}
