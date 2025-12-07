using UnityEngine;

public class Threat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseAgent agent))
        {
            if (other.TryGetComponent(out AgentStats stats))
            {
                stats.MarkAsDead();
            }

            Destroy(other.gameObject);
        }
    }
}
