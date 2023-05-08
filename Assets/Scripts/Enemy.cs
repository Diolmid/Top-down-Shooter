using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    [SerializeField] private float pathRefreshRate = 0.25f;

    private NavMeshAgent _navMesh;
    private Transform _target;

    private void Awake()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        while(_target != null && !dead)
        {
            var targetPosition = new Vector3(_target.position.x, 0f, _target.position.z);
            _navMesh.SetDestination(targetPosition);

            yield return new WaitForSeconds(pathRefreshRate);
        }
    }
}