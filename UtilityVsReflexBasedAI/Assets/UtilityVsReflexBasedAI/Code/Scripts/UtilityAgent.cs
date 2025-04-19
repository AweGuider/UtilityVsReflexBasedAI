using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UtilityAgent : BaseAgent
{
    [SerializeField] private float _avoidThreatWeight = 1.5f;
    [SerializeField] private float _seekCollectibleWeight = 1f;

    protected override void DecideAction()
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject[] threats = GameObject.FindGameObjectsWithTag("Threat");

        float threatScore = CalculateThreatUtility(threats);
        float collectibleScore = CalculateCollectibleUtility(collectibles);

        if (threatScore > collectibleScore)
        {
            MoveAwayFrom(FindClosest(threats));
        }
        else
        {
            MoveTowards(FindClosest(collectibles));
        }
    }

    private float CalculateThreatUtility(GameObject[] threats)
    {
        GameObject closest = FindClosest(threats);
        if (closest == null) return 0f;

        float distance = Vector3.Distance(transform.position, closest.transform.position);
        return (1f / (distance + 0.1f)) * _avoidThreatWeight;
    }

    private float CalculateCollectibleUtility(GameObject[] collectibles)
    {
        GameObject closest = FindClosest(collectibles);
        if (closest == null) return 0f;

        float distance = Vector3.Distance(transform.position, closest.transform.position);
        return (1f / (distance + 0.1f)) * _seekCollectibleWeight;
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
}
