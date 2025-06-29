using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UtilityAgent;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject utilityAgentPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int populationSize = 10;

    public List<AgentStats> allAgentStats = new();

    public List<UtilityGene> currentGenePool = new();

    [SerializeField] private int currentGeneration = 1;

    void Start()
    {
        //GeneratePopulation();
    }

    public void BeginEvaluation()
    {
        currentGeneration = 1;
        GeneratePopulation();
    }

    public void GeneratePopulation()
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
            GameObject agentObj = Instantiate(utilityAgentPrefab, spawn.position, Quaternion.identity);
            UtilityAgent agent = agentObj.GetComponent<UtilityAgent>();
            if (agent)
            {
                agent.Init(gene, i, currentGeneration);

                allAgentStats.Add(agent.GetComponent<AgentStats>());
            }
        }

        currentGeneration++;
    }

    private List<AgentStats> SelectTopPerformers(int topN)
    {
        // Sort descending by fitness
        allAgentStats.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        // Return the top N
        return allAgentStats.Take(topN).ToList();
    }
}
