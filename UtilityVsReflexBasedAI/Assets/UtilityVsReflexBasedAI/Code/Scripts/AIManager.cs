using System.Collections;
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
        public UtilitySpecs specs;
        public int amount = 1;
    }

    [System.Serializable]
    public class ReflexAgentSpawnInfo
    {
        public string name;
        public Transform spawnPoint;
        public int amount = 1;
    }

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
        SpawnAgents(_utilityAgentSpawns, _utilityAgentPrefab, _utilityAgentSpawnParent);
        SpawnAgents(_reflexAgentSpawns, _reflexAgentPrefab, _reflexAgentSpawnParent);
    }

    private void SpawnAgents(List<UtilityAgentSpawnInfo> spawnInfoList, GameObject agentPrefab, Transform parent)
    {
        foreach (var spawnInfo in spawnInfoList)
        {
            if (!spawnInfo.spawnPoint.gameObject.activeSelf) continue; // Skip if spawn point is not assigned

            for (int i = 0; i < spawnInfo.amount; i++)
            {
                Vector3 spawnPosition = spawnInfo.spawnPoint.position;
                spawnPosition += new Vector3(i * 1.5f, 0, 0); // Offset so they don't spawn at the same spot

                GameObject agent = Instantiate(agentPrefab, spawnPosition, Quaternion.identity, parent);
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
        foreach (var spawnInfo in spawnInfoList)
        {
            if (!spawnInfo.spawnPoint.gameObject.activeSelf) continue; // Skip if spawn point is not assigned

            for (int i = 0; i < spawnInfo.amount; i++)
            {
                Vector3 spawnPosition = spawnInfo.spawnPoint.position;
                spawnPosition += new Vector3(i * 1.5f, 0, 0); // Offset so they don't spawn at the same spot
                Instantiate(agentPrefab, spawnPosition, Quaternion.identity, parent);
            }
        }
    }

    [ContextMenu("Validate Spawn Points")]
    private void ValidateSpawnPoints()
    {
        for (int i = 0; i < _utilityAgentSpawns.Count; i++ )
        {
            UtilityAgentSpawnInfo spawnInfo = _utilityAgentSpawns[i];
            spawnInfo.name = "Utility Spawn Point " + (i + 1); // Unique name for each spawn point
            spawnInfo.spawnPoint.name = spawnInfo.name; // Set the name of the spawn point transform
        }

        for (int i = 0; i < _reflexAgentSpawns.Count; i++)
        {
            ReflexAgentSpawnInfo spawnInfo = _reflexAgentSpawns[i];
            spawnInfo.name = "Reflext Spawn Point " + (i + 1); // Unique name for each spawn point
            spawnInfo.spawnPoint.name = spawnInfo.name; // Set the name of the spawn point transform
        }
    }



    [ContextMenu("Reset Spawn Points Specs")]
    private void ResetSpawnPointsSpecs()
    {
        for (int i = 0; i < _utilityAgentSpawns.Count; i++)
        {
            UtilityAgentSpawnInfo spawnInfo = _utilityAgentSpawns[i];
            spawnInfo.specs = new UtilitySpecs(); // Reset to default specs
            spawnInfo.amount = 1; // Reset to default amount
        }

        for (int i = 0; i < _reflexAgentSpawns.Count; i++)
        {
            ReflexAgentSpawnInfo spawnInfo = _reflexAgentSpawns[i];
            spawnInfo.amount = 1; // Reset to default amount
        }
    }
}

