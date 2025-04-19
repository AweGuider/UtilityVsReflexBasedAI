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
    [SerializeField] protected Rigidbody _rb;

    protected virtual void Update()
    {
        SenseEnvironment();
        DecideAction();
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
	void Start()
	{
		Init();
	}
	void OnValidate()
	{
		if (_initOnValidate) Init();
	}
}
