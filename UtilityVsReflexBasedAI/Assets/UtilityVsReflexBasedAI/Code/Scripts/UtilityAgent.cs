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

    [SerializeField] private float _avoidThreatWeight = 1f;
    [SerializeField] private float _seekCollectibleWeight = 1f;


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

        if (threatScore > collectibleScore)
        {
            _currentTarget = FindClosest(threats);
            MoveAwayFrom(_currentTarget);
        }
        else
        {
            _currentTarget = FindClosest(collectibles);
            MoveTowards(_currentTarget);
        }


        UpdateScoreText(threatScore, collectibleScore);
    }

    private float CalculateThreatUtility(GameObject[] threats)
    {
        GameObject closest = FindClosest(threats);
        if (closest == null) return 0f;

        float distance = Vector3.Distance(transform.position, closest.transform.position);
        // Assume 15 units is your max relevant range
        float normalized = 1f - Mathf.Clamp01(distance / 15f);
        return normalized * _avoidThreatWeight;
    }

    private float CalculateCollectibleUtility(GameObject[] collectibles)
    {
        GameObject closest = FindClosest(collectibles);
        if (closest == null) return 0f;

        float distance = Vector3.Distance(transform.position, closest.transform.position);
        float normalized = 1f - Mathf.Clamp01(distance / 15f);
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
