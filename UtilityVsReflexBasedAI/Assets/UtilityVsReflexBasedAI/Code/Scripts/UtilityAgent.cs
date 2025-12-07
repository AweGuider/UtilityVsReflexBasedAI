using TMPro;
using UnityEngine;

public class UtilityAgent : BaseAgent
{
    [System.Serializable]
    public class UtilityGene
    {
        public float avoidThreatWeight = 1f;
        public float seekCollectibleWeight = 1f;
        public float minimumDifferenceThresholdBetweenWeights = 0.02f;
        public float maxRelevantDistance = 35f;
        public float threatProximityPenaltyRadius = 3.5f;
        public float threatProximityPenaltyWeight = 0.65f;

        public UtilityGene() { }

        public static UtilityGene Crossover(UtilityGene parentA, UtilityGene parentB)
        {
            UtilityGene child = new()
            {
                avoidThreatWeight = (Random.value < 0.5f) ? parentA.avoidThreatWeight : parentB.avoidThreatWeight,
                seekCollectibleWeight = (Random.value < 0.5f) ? parentA.seekCollectibleWeight : parentB.seekCollectibleWeight,
                minimumDifferenceThresholdBetweenWeights = (Random.value < 0.5f) ? parentA.minimumDifferenceThresholdBetweenWeights : parentB.minimumDifferenceThresholdBetweenWeights,
                maxRelevantDistance = (Random.value < 0.5f) ? parentA.maxRelevantDistance : parentB.maxRelevantDistance,
                threatProximityPenaltyRadius = (Random.value < 0.5f) ? parentA.threatProximityPenaltyRadius : parentB.threatProximityPenaltyRadius,
                threatProximityPenaltyWeight = (Random.value < 0.5f) ? parentA.threatProximityPenaltyWeight : parentB.threatProximityPenaltyWeight
            };

            return child;
        }

        public static UtilityGene Mutate(UtilityGene gene, float mutationRate)
        {
            UtilityGene mutated = gene;

            float MutateValue(float value, float min, float max)
            {
                if (Random.value < mutationRate)
                {
                    float delta = Random.Range(-0.2f, 0.2f);
                    return Mathf.Clamp(value + delta, min, max);
                }
                return value;
            }

            mutated.avoidThreatWeight = MutateValue(mutated.avoidThreatWeight, 0.1f, 5f);
            mutated.seekCollectibleWeight = MutateValue(mutated.seekCollectibleWeight, 0.1f, 5f);
            mutated.minimumDifferenceThresholdBetweenWeights = MutateValue(mutated.minimumDifferenceThresholdBetweenWeights, 0.01f, 0.5f);
            mutated.maxRelevantDistance = MutateValue(mutated.maxRelevantDistance, 5f, 50f);
            mutated.threatProximityPenaltyRadius = MutateValue(mutated.threatProximityPenaltyRadius, 1f, 10f);
            mutated.threatProximityPenaltyWeight = MutateValue(mutated.threatProximityPenaltyWeight, 0f, 2f);

            return mutated;
        }

        public override string ToString()
        {
            return $"Avoid: {avoidThreatWeight:F2}, Seek: {seekCollectibleWeight:F2}, ΔThresh: {minimumDifferenceThresholdBetweenWeights:F2}, MaxDist: {maxRelevantDistance:F2}, Radius: {threatProximityPenaltyRadius:F2}, PenaltyW: {threatProximityPenaltyWeight:F2}";
        }
    }

    private enum ActionType 
    { 
        None, 
        Avoiding, 
        Collecting 
    }

    private ActionType _lastAction = ActionType.None;

    [SerializeField] private UtilityGene _gene;

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _collectedText;
    private GameObject _currentTarget;

    public void Init(UtilityGene gene, int agentId = -1, int generation = -1)
    {
        _gene = gene;
        _agentStats.gene = gene;
        if (agentId != -1) _agentStats.agentId = agentId;
        if (generation != -1) _agentStats.generation = generation;

        _collectedText.SetText("");
    }

    public override void AddScore(int value)
    {
        base.AddScore(value);

        string textToSet = _score > 0 ? $"Collected: {_score}" : "";
        _collectedText.SetText(textToSet);
    }

    protected override void DecideAction()
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject[] threats = GameObject.FindGameObjectsWithTag("Threat");

        float threatScore = CalculateThreatUtility(threats);
        float collectibleScore = CalculateCollectibleUtility(collectibles, threats);

        float difference = Mathf.Abs(threatScore - collectibleScore);
        if (difference < _gene.minimumDifferenceThresholdBetweenWeights)
        {
            switch (_lastAction)
            {
                case ActionType.Avoiding:
                    MoveAwayFrom(_currentTarget);
                    break;
                case ActionType.Collecting:
                    MoveTowards(_currentTarget);
                    break;
                default:
                    _agentStats.SwitchBehavior("Idle");
                    break;
            }

            UpdateScoreText(threatScore, collectibleScore);
            return;
        }

        if (threatScore > collectibleScore)
        {
            if (_lastAction != ActionType.Avoiding)
            {
                _currentTarget = FindClosest(threats);
                _lastAction = ActionType.Avoiding;

                _agentStats.SwitchBehavior("Avoiding");
            }

            MoveAwayFrom(_currentTarget);
        }
        else
        {
            if (_lastAction != ActionType.Collecting)
            {
                _currentTarget = FindClosest(collectibles);
                _lastAction = ActionType.Collecting;

                _agentStats.SwitchBehavior("Collecting");
            }

            if (_currentTarget == null || !_currentTarget.activeInHierarchy)
            {
                _lastAction = ActionType.None;
                _currentTarget = null;
                _agentStats.SwitchBehavior("Idle");
            }
            else
            {
                MoveTowards(_currentTarget);
            }
        }

        UpdateScoreText(threatScore, collectibleScore);
    }

    private float CalculateThreatUtility(GameObject[] threats)
    {
        GameObject closest = FindClosest(threats);
        if (closest == null) return 0f;

        return CalculateNormalizedDistanceToClosest(closest, _gene.avoidThreatWeight);
    }

    private float CalculateCollectibleUtility(GameObject[] collectibles, GameObject[] threats)
    {
        GameObject closest = FindClosest(collectibles);
        if (closest == null) return 0f;

        float score = CalculateNormalizedDistanceToClosest(closest, _gene.seekCollectibleWeight);

        // Check if threats are nearby the collectible
        foreach (GameObject threat in threats)
        {
            if (Vector3.Distance(closest.transform.position, threat.transform.position) <= _gene.threatProximityPenaltyRadius)
            {
                score *= _gene.threatProximityPenaltyWeight;
                break;
            }
        }

        return score;
    }

    private float CalculateNormalizedDistanceToClosest(GameObject closest, float weight)
    {
        float distance = Vector3.Distance(transform.position, closest.transform.position);

        float normalized = 1f - Mathf.Clamp01(distance / _gene.maxRelevantDistance);
        return normalized * weight;
    }

    private void MoveTowards(GameObject target)
    {
        if (target == null) return;



        Vector3 dir = (target.transform.position - transform.position).normalized;
        _rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    private void MoveAwayFrom(GameObject target)
    {
        if (target == null) return;
        Vector3 dir = (transform.position - target.transform.position).normalized;
        _rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    private GameObject FindClosest(GameObject[] objs)
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var obj in objs)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                closest = obj;
                minDist = dist;
            }
        }

        return closest;
    }

    private void UpdateScoreText(float threat, float collect)
    {
        if (_scoreText == null) return;
        _scoreText.text = $"T: {threat:F2}\nC: {collect:F2}";

        if (_currentTarget != null && _lastAction == ActionType.Collecting)
        {
            float dist = Vector3.Distance(transform.position, _currentTarget.transform.position);
            _scoreText.text += $"\nD: {dist:F1}";
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //Debug.Log($"{gameObject.name} summary: Collecting {_agentStats.collectingTime:F1}s, Avoiding {_agentStats.avoidingTime:F1}s");
    }

    private void OnDrawGizmos()
    {
        if (_currentTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
        }
    }
}
