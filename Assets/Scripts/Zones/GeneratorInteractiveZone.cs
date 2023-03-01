using UnityEngine;
using UnityEngine.Events;

public class GeneratorInteractiveZone : MonoBehaviour
{
    public UnityAction OpenedText;
    public UnityAction ClosedText;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithGenerator(gameObject.GetComponentInParent<Generator>(), true);
            OpenedText?.Invoke();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.HandleInteractiveWithGenerator(null, false);
            ClosedText?.Invoke();
        }
    }
}
