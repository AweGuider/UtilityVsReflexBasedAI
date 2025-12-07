using System.IO;
using UnityEngine;

public static class BestAgentLogger
{
    private static string _directoryPath = "Assets/UtilityVsReflexBasedAI/Logs";
    private static string _filePath = Path.Combine(_directoryPath, "best_agents.csv");
    private static bool _hasWrittenHeader = false;

    public static void LogBestAgent(AgentStats best)
    {
        if (best == null)
        {
            Debug.LogWarning("BestAgentLogger.LogBestAgent called with null AgentStats.");
            return;
        }

        if (!_hasWrittenHeader)
        {
            WriteHeader();
            _hasWrittenHeader = true;
        }

        var gene = best.gene;
        string line = string.Join(",",
            best.generation,
            best.agentId,
            best.fitness.ToString("F3"),
            best.collectiblesCollected,
            best.timeAlive.ToString("F2"),
            best.collectingTime.ToString("F2"),
            best.avoidingTime.ToString("F2"),
            best.firstCollectTime >= 0f ? best.firstCollectTime.ToString("F2") : "N/A",
            gene != null ? gene.avoidThreatWeight.ToString("F3") : "N/A",
            gene != null ? gene.seekCollectibleWeight.ToString("F3") : "N/A",
            gene != null ? gene.minimumDifferenceThresholdBetweenWeights.ToString("F3") : "N/A",
            gene != null ? gene.maxRelevantDistance.ToString("F3") : "N/A",
            gene != null ? gene.threatProximityPenaltyRadius.ToString("F3") : "N/A",
            gene != null ? gene.threatProximityPenaltyWeight.ToString("F3") : "N/A"
        );

        File.AppendAllText(_filePath, line + "\n");
    }

    private static void WriteHeader()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }

        string header =
            "Generation,AgentId,Fitness,Collectibles,TimeAlive,CollectingTime,AvoidingTime,TimeToFirstCollect," +
            "AvoidWeight,SeekWeight,MinDifference,MaxRelevantDistance,ThreatRadius,ThreatWeight";

        File.WriteAllText(_filePath, header + "\n");
    }
}
