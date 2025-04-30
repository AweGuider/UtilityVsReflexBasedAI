using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseAgent agent))
        {
            AgentStats stats = other.GetComponent<AgentStats>();
            if (stats != null)
            {
                stats.MarkAsDead();
            }

            Destroy(other.gameObject);
        }
    }
}
