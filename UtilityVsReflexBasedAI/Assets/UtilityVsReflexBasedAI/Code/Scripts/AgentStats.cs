using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    private float _spawnTime;
    private float _deathTime;
    private bool _isAlive = true;

    public float TimeAlive => _isAlive ? Time.time - _spawnTime : _deathTime - _spawnTime;

    private void Start()
    {
        _spawnTime = Time.time;
    }

    public void MarkAsDead()
    {
        if (!_isAlive) return;

        _deathTime = Time.time;
        _isAlive = false;
        Debug.Log($"{gameObject.name} died at {TimeAlive:F2} seconds.");
    }
}

