using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseAgent agent))
        {
            Destroy(other.gameObject);
            // Optional: Log or notify a system (e.g., “Agent destroyed by threat”)
        }
    }
}
