using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseAgent : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    public float moveSpeed => _moveSpeed;
    [SerializeField] private float _detectionRadius = 10f;
    public float detectionRadius => _detectionRadius;

    [SerializeField] private bool _isFirstCollected;

    protected int _score = 0;
    public int score => _score;

    protected AgentStats _agentStats;

    [SerializeField] protected Rigidbody _rb;

    protected virtual void Update()
    {
        SenseEnvironment();
        DecideAction();
    }

    public void AddScore(int value)
    {
        _score += value;

        if (!_isFirstCollected && TryGetComponent(out AgentStats stats))
        {
            _isFirstCollected = true;
            stats.RegisterFirstCollect();
        }

        Debug.Log($"{gameObject.name} score: {_score}");
    }

    protected abstract void DecideAction();

    protected virtual void SenseEnvironment()
    {
        // Optional: OverlapSphere, raycasts, or tagging system
    }

    [Tooltip("If you want call Init() on OnValidate(), check it")]
	[SerializeField] private bool _initOnValidate;

    protected virtual private void Init()
	{
        _rb = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
	{
		Init();
        _agentStats = GetComponent<AgentStats>();

    }

    protected virtual void OnDestroy()
    {
        string message = "";
        if (_agentStats.firstCollectTime <= 0f)
        {
            message += $"{gameObject.name} did not collect first item.\n";
        }
        else
        {
            message += $"{gameObject.name} collected first item at {_agentStats.firstCollectTime:F2} seconds.\n";
        }

        //message += $"{gameObject.name} was alive for {_agentStats.timeAlive:F2} seconds.";

        Debug.Log($"{message}");
    }

    void OnValidate()
	{
		if (_initOnValidate) Init();
	}
}
