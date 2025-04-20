using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseAgent agent))
        {
            if (agent != null)
            {
                agent.AddScore(1);
            }

            Destroy(gameObject);
            // Optional: Notify a manager or increment a score
        }
    }
}
