using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDemoLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            Application.Quit();
        }
    }
}
