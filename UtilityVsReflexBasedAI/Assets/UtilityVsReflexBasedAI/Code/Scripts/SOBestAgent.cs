using UnityEngine;

[CreateAssetMenu(fileName = "SOBestAgent", menuName = "AI/SOBestAgent")]
public class SOBestAgent : ScriptableObject
{
    [Header("Meta")]
    public int generation;
    public int agentId;
    public float fitness;

    [Header("Performance")]
    public int collectiblesCollected;
    public float timeAlive;
    public float collectingTime;
    public float avoidingTime;
    public float firstCollectTime;

    [Header("Gene")]
    public float avoidThreatWeight;
    public float seekCollectibleWeight;
    public float minimumDifferenceThresholdBetweenWeights;
    public float maxRelevantDistance;
    public float threatProximityPenaltyRadius;
    public float threatProximityPenaltyWeight;

    public void UpdateFromStats(AgentStats stats)
    {
        if (stats == null) return;
        var gene = stats.gene;

        generation = stats.generation;
        agentId = stats.agentId;
        fitness = stats.fitness;

        collectiblesCollected = stats.collectiblesCollected;
        timeAlive = stats.timeAlive;
        collectingTime = stats.collectingTime;
        avoidingTime = stats.avoidingTime;
        firstCollectTime = stats.firstCollectTime;

        if (gene != null)
        {
            avoidThreatWeight = gene.avoidThreatWeight;
            seekCollectibleWeight = gene.seekCollectibleWeight;
            minimumDifferenceThresholdBetweenWeights = gene.minimumDifferenceThresholdBetweenWeights;
            maxRelevantDistance = gene.maxRelevantDistance;
            threatProximityPenaltyRadius = gene.threatProximityPenaltyRadius;
            threatProximityPenaltyWeight = gene.threatProximityPenaltyWeight;
        }
    }
}
