using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexAgent : BaseAgent
{
    protected override void DecideAction()
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject[] threats = GameObject.FindGameObjectsWithTag("Threat");

        GameObject closest = FindClosest(collectibles);
        GameObject danger = FindClosest(threats);

        if (danger != null && Vector3.Distance(transform.position, danger.transform.position) < 3f)
        {
            // Move away
            Vector3 dir = (transform.position - danger.transform.position).normalized;
            _rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
        }
        else if (closest != null)
        {
            // Move toward collectible
            Vector3 dir = (closest.transform.position - transform.position).normalized;
            _rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
        }
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
