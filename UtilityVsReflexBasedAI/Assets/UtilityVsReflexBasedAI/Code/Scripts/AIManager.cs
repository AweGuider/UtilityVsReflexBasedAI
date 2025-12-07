using System.Collections.Generic;
using UnityEngine;
using static UtilityAgent;

public class AIManager : MonoBehaviour
{
    [System.Serializable]
    public class UtilityAgentSpawnInfo
    {
        public string name;
        public Transform spawnPoint;
        public UtilityGene specs;
        public int amount = 1;
    }

    [System.Serializable]
    public class ReflexAgentSpawnInfo
    {
        public string name;
        public Transform spawnPoint;
        public int amount = 1;
    }

    [SerializeField] private bool _useGeneticMode;
    [SerializeField] private PopulationManager _populationManager;

    [Header("Utility Agent Spawn Info")]
    [SerializeField] private Transform _utilityAgentSpawnParent;
    [SerializeField] private List<UtilityAgentSpawnInfo> _utilityAgentSpawns;

    [Header("Reflex Agent Spawn Info")]
    [SerializeField] private Transform _reflexAgentSpawnParent;
    [SerializeField] private List<ReflexAgentSpawnInfo> _reflexAgentSpawns;

    [Header("Agent Prefabs")]
    [SerializeField] private GameObject _utilityAgentPrefab;
    [SerializeField] private GameObject _reflexAgentPrefab;

    private void Start()
    {
        if (_populationManager == null)
        {
            _populationManager = FindObjectOfType<PopulationManager>();
        }

        if (_useGeneticMode && _populationManager != null)
        {
            _populationManager.BeginEvaluation();
        }
        else
        {
            SpawnAgents(_utilityAgentSpawns, _utilityAgentPrefab, _utilityAgentSpawnParent);
            SpawnAgents(_reflexAgentSpawns, _reflexAgentPrefab, _reflexAgentSpawnParent);
        }
    }

    private void SpawnAgents(List<UtilityAgentSpawnInfo> spawnInfoList, GameObject agentPrefab, Transform parent)
    {
        for (int j = 0; j < spawnInfoList.Count; j++)
        {
            var spawnInfo = spawnInfoList[j];

            if (!spawnInfo.spawnPoint.gameObject.activeSelf) continue;

            for (int i = 0; i < spawnInfo.amount; i++)
            {
                Vector3 spawnPosition = spawnInfo.spawnPoint.position;
                spawnPosition += new Vector3(i * 1.5f, 0, 0);

                GameObject agent = Instantiate(agentPrefab, spawnPosition, Quaternion.identity, parent);
                agent.transform.name = "UtilityAgent " + j;

                UtilityAgent utilityAgent = agent.GetComponent<UtilityAgent>();
                if (utilityAgent != null)
                {
                    utilityAgent.Init(spawnInfo.specs);
                }
            }
        }
    }

    private void SpawnAgents(List<ReflexAgentSpawnInfo> spawnInfoList, GameObject agentPrefab, Transform parent)
    {
        for (int j = 0; j < spawnInfoList.Count; j++)
        {
            var spawnInfo = spawnInfoList[j];

            if (!spawnInfo.spawnPoint.gameObject.activeSelf) continue;

            for (int i = 0; i < spawnInfo.amount; i++)
            {
                Vector3 spawnPosition = spawnInfo.spawnPoint.position;
                spawnPosition += new Vector3(i * 1.5f, 0, 0); // Offset so they don't spawn at the same spot
                GameObject agent = Instantiate(agentPrefab, spawnPosition, Quaternion.identity, parent);

                agent.transform.name = "ReflextAgent " + j; 
            }
        }
    }

    [ContextMenu("Validate Spawn Points")]
    private void ValidateSpawnPoints()
    {
        for (int i = 0; i < _utilityAgentSpawns.Count; i++ )
        {
            UtilityAgentSpawnInfo spawnInfo = _utilityAgentSpawns[i];
            spawnInfo.name = "Utility Spawn Point " + (i + 1);
            spawnInfo.spawnPoint.name = spawnInfo.name;
        }

        for (int i = 0; i < _reflexAgentSpawns.Count; i++)
        {
            ReflexAgentSpawnInfo spawnInfo = _reflexAgentSpawns[i];
            spawnInfo.name = "Reflext Spawn Point " + (i + 1);
            spawnInfo.spawnPoint.name = spawnInfo.name;
        }
    }

    [ContextMenu("Reset Spawn Points Specs")]
    private void ResetSpawnPointsSpecs()
    {
        for (int i = 0; i < _utilityAgentSpawns.Count; i++)
        {
            UtilityAgentSpawnInfo spawnInfo = _utilityAgentSpawns[i];
            spawnInfo.specs = new UtilityGene();
            spawnInfo.amount = 1;
        }

        for (int i = 0; i < _reflexAgentSpawns.Count; i++)
        {
            ReflexAgentSpawnInfo spawnInfo = _reflexAgentSpawns[i];
            spawnInfo.amount = 1;
        }
    }
}
