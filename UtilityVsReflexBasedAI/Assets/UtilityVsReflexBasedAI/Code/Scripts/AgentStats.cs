using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    private float _spawnTime;
    private float _deathTime;
    private bool _isAlive = true;

    private float _collectingTime = 0f;
    public float collectingTime => _collectingTime;
    private float _avoidingTime = 0f;
    public float avoidingTime => _avoidingTime;

    private float _lastStateChangeTime;
    private string _currentState = "None";

    private float _firstCollectTime = -1f;
    public float firstCollectTime => _firstCollectTime;

    public float timeAlive => _isAlive ? Time.time - _spawnTime : _deathTime - _spawnTime;

    private void Start()
    {
        _spawnTime = Time.time;
    }

    public void SwitchBehavior(string newState)
    {
        float now = Time.time;

        // Store previous state's duration
        float timeInState = now - _lastStateChangeTime;
        switch (_currentState)
        {
            case "Collecting":
                _collectingTime += timeInState;
                break;
            case "Avoiding":
                _avoidingTime += timeInState;
                break;
        }

        // Switch to new state
        _currentState = newState;
        _lastStateChangeTime = now;
    }

    public void RegisterFirstCollect()
    {
        if (_firstCollectTime < 0f)
        {
            _firstCollectTime = Time.time - _spawnTime;
            Debug.Log($"{gameObject.name} collected first item at {firstCollectTime:F2} seconds.");
        }
    }

    public void MarkAsDead()
    {
        if (!_isAlive) return;

        _deathTime = Time.time;
        _isAlive = false;
        Debug.Log($"{gameObject.name} died at {timeAlive:F2} seconds.");
    }
}

