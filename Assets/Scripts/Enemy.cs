using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chesing, Attacking}

    [SerializeField] private float pathRefreshRate = 0.25f;

    private float _damage = 1;
    private float _attackDistanceThreshold = 0.5f;
    private float _timeBetweenAttack = 1;
    private float _nextAttackTime;
    private float _myCollisionRadius;
    private float _targetCollisionRadius;
    private bool _hasTarget;
    private bool _hasPlayerOnScene;

    private NavMeshAgent _navMesh;
    private Transform _target;
    private LivingEntity _targetEntity;

    private State _currentState;

    private void Awake()
    {
        _navMesh = GetComponent<NavMeshAgent>();

        _hasPlayerOnScene = GameObject.FindGameObjectWithTag("Player") != null;
        if (_hasPlayerOnScene)
        {
            _hasTarget = true;
            _targetEntity = _target.GetComponent<LivingEntity>();
            _target = GameObject.FindGameObjectWithTag("Player").transform;

            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;
            _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (_hasPlayerOnScene)
        {
            _currentState = State.Chesing;  
            _targetEntity.OnDeath += OnTargetDeath;
        }

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (_hasTarget)
        {
            if(Time.time > _nextAttackTime)
            {
                float sqrDisToTarget = (_target.position - transform.position).sqrMagnitude;
                if (sqrDisToTarget < Mathf.Pow(_attackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius, 2))
                {
                    _nextAttackTime = Time.time + _timeBetweenAttack;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    private void OnTargetDeath()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }

    private IEnumerator Attack()
    {
        _currentState = State.Attacking;
        _navMesh.enabled = false;

        var originalPosition = transform.position;
        var directionToTarget = (_target.position - transform.position).normalized;
        var attackPosition = _target.position - directionToTarget * (_myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;

        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                _targetEntity.TakeDamage(_damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        _currentState = State.Chesing;
        _navMesh.enabled = true;
    }

    private IEnumerator UpdatePath()
    {
        while(_hasTarget && !dead)
        {
            if(_currentState == State.Chesing)
            {
                var directionToTarget = (_target.position - transform.position).normalized;
                var targetPosition = _target.position - directionToTarget * (_myCollisionRadius + _targetCollisionRadius + _attackDistanceThreshold / 2);
                _navMesh.SetDestination(targetPosition);
            }

            yield return new WaitForSeconds(pathRefreshRate);
        }
    }
}