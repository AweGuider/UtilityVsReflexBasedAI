using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UtilityAgent;

public class PopulationManager : MonoBehaviour
{

    [SerializeField] private GameObject utilityAgentPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int populationSize = 10;

    public List<UtilityGene> currentGenePool = new();

    void Start()
    {
        GenerateRandomPopulation();
    }

    void GenerateRandomPopulation()
    {
        currentGenePool.Clear();

        for (int i = 0; i < populationSize; i++)
        {
            UtilityGene gene = new()
            {
                avoidThreatWeight = Random.Range(0.1f, 2f),
                seekCollectibleWeight = Random.Range(1f, 3f),
                minimumDifferenceThresholdBetweenWeights = Random.Range(0.01f, 0.1f),
                maxRelevantDistance = Random.Range(10f, 35f),
                threatProximityPenaltyRadius = Random.Range(2f, 5f),
                threatProximityPenaltyWeight = Random.Range(0.2f, 1f)
            };

            currentGenePool.Add(gene);

            // Spawn agent
            Transform spawn = spawnPoints[i % spawnPoints.Length];
            GameObject agentGO = Instantiate(utilityAgentPrefab, spawn.position, Quaternion.identity);

            if (agentGO.TryGetComponent(out UtilityAgent agent))
            {
                agent.Init(gene);
            }
        }
    }
}
