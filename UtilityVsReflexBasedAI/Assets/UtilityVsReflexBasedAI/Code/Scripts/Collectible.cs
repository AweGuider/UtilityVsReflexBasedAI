using System;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public event Action<Collectible> OnCollected;
    public event Action<Collectible> OnDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseAgent agent))
        {
            if (agent != null)
            {
                agent.AddScore(1);
            }

            GetCollected();
        }
    }

    private void GetCollected()
    {
        gameObject.SetActive(false);

        OnCollected?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
}
