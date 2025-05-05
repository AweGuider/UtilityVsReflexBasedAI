using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UtilityAgent : BaseAgent
{
    [System.Serializable]
    public class UtilitySpecs
    {
        public float avoidThreatWeight = 1f;
        public float seekCollectibleWeight = 1f;
        public float _minimumDifferenceThresholdBetweenWeights = 0.02f;

        public float _maxRelevantDistance = 35f;
        public float _threatProximityPenaltyRadius = 3.5f;
        public float _threatProximityPenaltyWeight = 0.65f;
    }

    private enum ActionType 
    { 
        None, 
        Avoiding, 
        Collecting 
    }

    private ActionType _lastAction = ActionType.None;

    [SerializeField] private float _avoidThreatWeight = 1f;
    [SerializeField] private float _seekCollectibleWeight = 1f;
    [SerializeField] private float _minimumDifferenceThresholdBetweenWeights = 0.02f;

    [SerializeField] private float _maxRelevantDistance = 35f;

    [SerializeField] private float _threatProximityPenaltyRadius = 3.5f;
    [SerializeField] private float _threatProximityPenaltyWeight = 0.65f;

    [SerializeField] private TextMeshProUGUI _scoreText;
    private GameObject _currentTarget;

    public void Init(UtilitySpecs utilitySpecs)
    {
        _avoidThreatWeight = utilitySpecs.avoidThreatWeight;
        _seekCollectibleWeight = utilitySpecs.seekCollectibleWeight;
        _minimumDifferenceThresholdBetweenWeights = utilitySpecs._minimumDifferenceThresholdBetweenWeights;
        _maxRelevantDistance = utilitySpecs._maxRelevantDistance;
        _threatProximityPenaltyRadius = utilitySpecs._threatProximityPenaltyRadius;
        _threatProximityPenaltyWeight = utilitySpecs._threatProximityPenaltyWeight;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void DecideAction()
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject[] threats = GameObject.FindGameObjectsWithTag("Threat");

        float threatScore = CalculateThreatUtility(threats);
        float collectibleScore = CalculateCollectibleUtility(collectibles, threats);

        float difference = Mathf.Abs(threatScore - collectibleScore);
        if (difference < _minimumDifferenceThresholdBetweenWeights)
        {
            // Difference too small — keep doing what we were doing
            switch (_lastAction)
            {
                case ActionType.Avoiding:
                    MoveAwayFrom(_currentTarget);
                    break;
                case ActionType.Collecting:
                    MoveTowards(_currentTarget);
                    break;
                default:
                    // Optionally idle or choose a default behavior
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
                // Target was collected or destroyed
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

        return CalculateNormalizedDistanceToClosest(closest, _avoidThreatWeight);
    }

    private float CalculateCollectibleUtility(GameObject[] collectibles, GameObject[] threats)
    {
        GameObject closest = FindClosest(collectibles);
        if (closest == null) return 0f;

        float score = CalculateNormalizedDistanceToClosest(closest, _seekCollectibleWeight);

        // Check if threats are nearby the collectible
        foreach (GameObject threat in threats)
        {
            if (Vector3.Distance(closest.transform.position, threat.transform.position) <= _threatProximityPenaltyRadius)
            {
                score *= _threatProximityPenaltyWeight; // Reduce utility score
                break;
            }
        }

        return score;
    }

    private float CalculateNormalizedDistanceToClosest(GameObject closest, float weight)
    {
        float distance = Vector3.Distance(transform.position, closest.transform.position);

        float normalized = 1f - Mathf.Clamp01(distance / _maxRelevantDistance);
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
        Debug.Log($"{gameObject.name} summary: Collecting {_agentStats.collectingTime:F1}s, Avoiding {_agentStats.avoidingTime:F1}s");
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
