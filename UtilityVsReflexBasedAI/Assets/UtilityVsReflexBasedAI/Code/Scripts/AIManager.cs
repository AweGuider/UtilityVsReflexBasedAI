using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [System.Serializable]
    public class AgentSpawnInfo
    {
        public string name;
        public Transform spawnPoint;
        public int amount = 1;
    }

    [Header("Utility Agent Spawn Info")]
    [SerializeField] private Transform _utilityAgentSpawnParent;
    [SerializeField] private List<AgentSpawnInfo> _utilityAgentSpawns;

    [Header("Reflex Agent Spawn Info")]
    [SerializeField] private Transform _reflexAgentSpawnParent;
    [SerializeField] private List<AgentSpawnInfo> _reflexAgentSpawns;

    [Header("Agent Prefabs")]
    [SerializeField] private GameObject _utilityAgentPrefab;
    [SerializeField] private GameObject _reflexAgentPrefab;

    private void Start()
    {
        SpawnAgents(_utilityAgentSpawns, _utilityAgentPrefab, _utilityAgentSpawnParent);
        SpawnAgents(_reflexAgentSpawns, _reflexAgentPrefab, _reflexAgentSpawnParent);
    }

    private void SpawnAgents(List<AgentSpawnInfo> spawnInfoList, GameObject agentPrefab, Transform parent)
    {
        foreach (var spawnInfo in spawnInfoList)
        {
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
            AgentSpawnInfo spawnInfo = _utilityAgentSpawns[i];
            spawnInfo.name = "Utility Point " + (i + 1); // Unique name for each spawn point
            spawnInfo.spawnPoint.name = spawnInfo.name; // Set the name of the spawn point transform
        }

        for (int i = 0; i < _reflexAgentSpawns.Count; i++)
        {
            AgentSpawnInfo spawnInfo = _reflexAgentSpawns[i];
            spawnInfo.name = "Reflext Point " + (i + 1); // Unique name for each spawn point
            spawnInfo.spawnPoint.name = spawnInfo.name; // Set the name of the spawn point transform
        }
    }
}

