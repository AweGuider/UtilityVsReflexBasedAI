using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MetricsLogger
{
    private static string _filePath = "Assets/UtilityVsReflexBasedAI/Logs/agent_metrics.csv";
    private static bool _hasWrittenHeader = false;

    public static void LogAgentMetrics(AgentStats stats)
    {
        if (!_hasWrittenHeader)
        {
            WriteHeader();
            _hasWrittenHeader = true;
        }

        string line = string.Join(",",
            stats.agentId,
            stats.generation,
            stats.gene.avoidThreatWeight,
            stats.gene.seekCollectibleWeight,
            stats.gene.minimumDifferenceThresholdBetweenWeights,
            stats.gene.maxRelevantDistance,
            stats.gene.threatProximityPenaltyRadius,
            stats.gene.threatProximityPenaltyWeight,
            stats.collectiblesCollected,
            stats.timeAlive.ToString("F2"),
            stats.collectingTime.ToString("F2"),
            stats.avoidingTime.ToString("F2"),
            stats.firstCollectTime >= 0 ? stats.firstCollectTime.ToString("F2") : "N/A",
            stats.fitness.ToString("F3")
        );

        File.AppendAllText(_filePath, line + "\n");
    }

    private static void WriteHeader()
    {
        string header = "AgentID,Generation,AvoidWeight,SeekWeight,MinDifference,MaxRelevantDist,ThreatRadius,ThreatWeight,Collectibles,TimeAlive,CollectingTime,AvoidingTime,TimeToFirstCollect,Fitness";
        File.WriteAllText(_filePath, header + "\n");
    }
}

