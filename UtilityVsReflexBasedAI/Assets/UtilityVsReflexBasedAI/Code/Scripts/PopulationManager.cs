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
    [SerializeField] private SOBestAgent _bestAgent;

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _populationSize;

    public List<AgentStats> allAgentStats = new();

    [SerializeField] private List<UtilityGene> genesPool = new();

    [SerializeField] private int _aliveAgents;
    [SerializeField] private int _currentGeneration = 1;
    [SerializeField] private int _maxGenerations = 100;

    [SerializeField] private float mutationRate = 0.1f;

    public static event Action<int> OnNewGenerationCreated;

    public void BeginEvaluation()
    {
        _currentGeneration = 1;

        _populationSize = Mathf.Min(_populationSize, _spawnPoints.Length);
        GenerateInitialPopulation();

        SimulationTimer.OnTimeLimitReached += HandleGenerationTimeout;
        CollectibleController.AllCollectiblesCollected += HandleGenerationAllCollectiblesCollected;
    }

    public void GenerateInitialPopulation()
    {
        genesPool.Clear();

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

            genesPool.Add(gene);

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
        agent.OnAgentDied += HandleAgentDied;

        allAgentStats.Add(agent.GetComponent<AgentStats>());

        _aliveAgents++;
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

        // Create new genes pool
        genesPool.Clear();
        List<AgentStats> parents = SelectTopPerformers(5);
        if (parents.Count > 0)
        {
            AgentStats best = parents[0];

            BestAgentLogger.LogBestAgent(best);

            if (_bestAgent != null)
            {
                if (best.fitness > _bestAgent.fitness)
                {
                    _bestAgent.UpdateFromStats(best);
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(_bestAgent);
#endif
                }
            }
        }
        for (int i = 0; i < _populationSize; i++)
        {
            // Randomly pick two parents
            var parentA = parents[Random.Range(0, parents.Count)];
            var parentB = parents[Random.Range(0, parents.Count)];

            UtilityGene childGene = UtilityGene.Crossover(parentA.gene, parentB.gene);
            childGene = UtilityGene.Mutate(childGene, mutationRate);

            genesPool.Add(childGene);
        }

        //Debug.Log($"Next generation genes count: {nextGenerationGenes.Count}");

        foreach (AgentStats stat in allAgentStats)
        {
            if (stat != null)
            {
                stat.GetComponent<UtilityAgent>().OnAgentDied -= HandleAgentDied;
                Destroy(stat.gameObject);
            }
        }
        allAgentStats.Clear();
        _aliveAgents = 0;

        // Spawn new generation
        _currentGeneration++;
        for (int i = 0; i < _populationSize; i++)
        {
            SpawnAgent(genesPool[i], i);
        }

        SimulationTimer.ResetTimer();

        OnNewGenerationCreated?.Invoke(_currentGeneration);
    }

    private List<AgentStats> SelectTopPerformers(int topN)
    {
        allAgentStats.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        return allAgentStats.Take(topN).ToList();
    }

    public void HandleAgentDied(BaseAgent agent)
    {
        AgentStats stats = agent.GetComponent<AgentStats>();
        if (allAgentStats.Contains(stats))
        {
            allAgentStats.Remove(stats);
        }

        _aliveAgents--;
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
