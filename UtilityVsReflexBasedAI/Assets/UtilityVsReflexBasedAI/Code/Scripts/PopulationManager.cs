using AweDev.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UtilityAgent;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject _utilityAgentPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _populationSize = 10;

    public List<AgentStats> allAgentStats = new();

    public List<UtilityGene> currentGenePool = new();

    [SerializeField] private int _aliveAgents;
    [SerializeField] private int _currentGeneration = 1;
    [SerializeField] private int _maxGenerations = 100;

    [SerializeField] private float mutationRate = 0.1f;

    public static event Action<int> OnNewGenerationCreated;

    public void BeginEvaluation()
    {
        _currentGeneration = 1;

        _populationSize = Mathf.Min(_populationSize, _spawnPoints.Length);
        GeneratePopulation();

        SimulationTimer.OnTimeLimitReached += HandleGenerationTimeout;
        CollectibleController.AllCollectiblesCollected += HandleGenerationAllCollectiblesCollected;
    }

    public void GeneratePopulation()
    {
        currentGenePool.Clear();

        for (int i = 0; i < _populationSize; i++)
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
            SpawnAgent(gene, i);
        }

        OnNewGenerationCreated?.Invoke(_currentGeneration);
    }

    private void SpawnAgent(UtilityGene gene, int agentId)
    {
        Transform spawn = _spawnPoints[agentId % _spawnPoints.Length];
        GameObject agentObj = Instantiate(_utilityAgentPrefab, spawn.position, Quaternion.identity);
        UtilityAgent agent = agentObj.GetComponent<UtilityAgent>();

        agent.Init(gene, agentId, _currentGeneration);
        allAgentStats.Add(agent.GetComponent<AgentStats>());
        _aliveAgents++;

        // Subscribe to death event
        agent.OnAgentDied += HandleAgentDied;
    }

    private void EndGeneration()
    {
        if (_currentGeneration >= _maxGenerations)
        {
            Debug.Log("Evaluation complete.");
            SimulationTimer.OnTimeLimitReached -= HandleGenerationTimeout;
            CollectibleController.AllCollectiblesCollected -= HandleGenerationAllCollectiblesCollected;
            PauseGame.Pause();
            return;
        }

        List<AgentStats> parents = SelectTopPerformers(5);
        List<UtilityGene> nextGenerationGenes = new();

        for (int i = 0; i < _populationSize; i++)
        {
            // Randomly pick two parents
            var parentA = parents[Random.Range(0, parents.Count)];
            var parentB = parents[Random.Range(0, parents.Count)];

            UtilityGene childGene = UtilityGene.Crossover(parentA.gene, parentB.gene);
            childGene = UtilityGene.Mutate(childGene, mutationRate);

            nextGenerationGenes.Add(childGene);
        }

        Debug.Log($"Next generation genes count: {nextGenerationGenes.Count}");

        // Clear old agents (if needed)
        foreach (AgentStats stat in allAgentStats)
        {
            if (stat != null)
            {
                stat.GetComponent<UtilityAgent>().OnAgentDied -= HandleAgentDied;
                Destroy(stat.gameObject);
            }
        }
        allAgentStats.Clear();

        // Spawn new generation
        _currentGeneration++;
        _aliveAgents = 0;

        for (int i = 0; i < _populationSize; i++)
        {
            SpawnAgent(nextGenerationGenes[i], i);
        }

        SimulationTimer.ResetTimer();

        OnNewGenerationCreated?.Invoke(_currentGeneration);
    }

    private List<AgentStats> SelectTopPerformers(int topN)
    {
        // Sort descending by fitness
        allAgentStats.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        // Return the top N
        return allAgentStats.Take(topN).ToList();
    }

    public void HandleAgentDied(BaseAgent agent)
    {
        _aliveAgents--;

        AgentStats stats = agent.GetComponent<AgentStats>();
        if (allAgentStats.Contains(stats))
        {
            allAgentStats.Remove(stats);
        }

        if (_aliveAgents <= 0)
        {
            EndGeneration();
        }
    }

    private void HandleGenerationTimeout()
    {
        Debug.Log("Timer limit reached — forcing end of generation.");
        EndGeneration();
    }

    private void HandleGenerationAllCollectiblesCollected()
    {
        Debug.Log("All collectibles collected — forcing end of generation.");
        EndGeneration();
    }

    private void OnDestroy()
    {
        SimulationTimer.OnTimeLimitReached -= HandleGenerationTimeout;
        CollectibleController.AllCollectiblesCollected -= HandleGenerationAllCollectiblesCollected;
    }
}
