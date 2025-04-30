using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UtilityAgent : BaseAgent
{
    [System.Serializable]
    public class UtilitySpecs
    {
        public float avoidThreatWeight = 1f;
        public float seekCollectibleWeight = 1f;
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
    [SerializeField] private float _minimumDifferenceThresholdBetweenWeights = 0.05f;

    [SerializeField] private float _maxRelevantDistance = 20f;


    [SerializeField] private TextMeshProUGUI _scoreText;
    private GameObject _currentTarget;

    public void Init(UtilitySpecs utilitySpecs)
    {
        _avoidThreatWeight = utilitySpecs.avoidThreatWeight;
        _seekCollectibleWeight = utilitySpecs.seekCollectibleWeight;
    }

    protected override void DecideAction()
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject[] threats = GameObject.FindGameObjectsWithTag("Threat");

        float threatScore = CalculateThreatUtility(threats);
        float collectibleScore = CalculateCollectibleUtility(collectibles);

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
            }
            MoveAwayFrom(_currentTarget);
        }
        else
        {
            if (_lastAction != ActionType.Collecting)
            {
                _currentTarget = FindClosest(collectibles);
                _lastAction = ActionType.Collecting;
            }
            MoveTowards(_currentTarget);
        }

        UpdateScoreText(threatScore, collectibleScore);
    }

    private float CalculateThreatUtility(GameObject[] threats)
    {
        GameObject closest = FindClosest(threats);
        if (closest == null) return 0f;

        return CalculateNormalizedDistanceToClosest(closest);
    }

    private float CalculateCollectibleUtility(GameObject[] collectibles)
    {
        GameObject closest = FindClosest(collectibles);
        if (closest == null) return 0f;

        return CalculateNormalizedDistanceToClosest(closest);
    }

    private float CalculateNormalizedDistanceToClosest(GameObject closest)
    {
        float distance = Vector3.Distance(transform.position, closest.transform.position);

        // Assume 15 units is your max relevant range
        float normalized = 1f - Mathf.Clamp01(distance / _maxRelevantDistance);
        return normalized * _seekCollectibleWeight;
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
