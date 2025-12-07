using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private List<Collectible> _collectibles = new();
    [SerializeField] private int _count = 0;

    public static event Action AllCollectiblesCollected;

    void Start()
    {
        PopulationManager.OnNewGenerationCreated += HandleNewGenerationCreated;

        if (_collectibles.Count == 0)
        {
            _collectibles = FindObjectsOfType<Collectible>().ToList();
            _count = _collectibles.Count;
            Debug.Log($"Amount of active Collectibles found in the scene: {_count}");
        }
        if (_count > 0 )
        {
            foreach (Collectible collectible in _collectibles)
            {
                collectible.OnCollected += HandleCollectibleCollected;
                collectible.OnDestroyed += HandleCollectibleDestroyed;
            }
        }

        ActivateCollectibles(true);
    }

    private void OnDestroy()
    {
        if (_collectibles.Count > 0)
        {
            foreach (Collectible collectible in _collectibles)
            {
                collectible.OnCollected -= HandleCollectibleCollected;
                collectible.OnDestroyed -= HandleCollectibleDestroyed;
            }
        }

        PopulationManager.OnNewGenerationCreated -= HandleNewGenerationCreated;
    }

    private void ActivateCollectibles(bool state)
    {
        foreach (Collectible collectible in _collectibles)
        {
            collectible.gameObject.SetActive(state);
        }
    }

    private void HandleNewGenerationCreated(int generation)
    {
        ActivateCollectibles(true);
    }

    private void HandleCollectibleCollected(Collectible collectible)
    {
        collectible.OnCollected -= HandleCollectibleCollected;
        collectible.OnDestroyed -= HandleCollectibleDestroyed;

        _count--;

        if (_count <= 0 )
        {
            AllCollectiblesCollected?.Invoke();
        }
    }

    private void HandleCollectibleDestroyed(Collectible collectible)
    {
        if (_collectibles != null && _collectibles.Count > 0 && _collectibles.Contains(collectible))
        {
            _collectibles.Remove(collectible);
        }
    }
}
