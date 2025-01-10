using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks; // Ensure UniTask is installed
using UnityEngine;
using UnityEngine.AI;
using System.Threading;

public class PatrollingState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;

    private Queue<Transform> _points = new();
    private NavMeshAgent _agent;
    private bool _isPatrolling = false;

    private CancellationTokenSource _cancellationTokenSource;

    // Constructor
    public PatrollingState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
        _agent = _data.NavMeshAgent; // Assuming Enemy has a NavMeshAgent
    }

    public void OnEnter()
    {
        if (!_agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a valid NavMesh. Patrolling cannot start.");
            return;
        }
        _points.Clear();
        _data.AnimatorComponent.SetBool("Walk", true);
        _data.StatusText.text = $"{_data.Name} - State: Patrolling";

        _cancellationTokenSource = new CancellationTokenSource();

        // Start patrolling
        if (!_isPatrolling)
        {
            _isPatrolling = true;
            Patrol(_cancellationTokenSource.Token).Forget();
        }

        Debug.Log("Player entered Patrolling State.");
    }

    public void OnUpdate()
    {
        // Switch to ChaseState if the enemy can chase
        if (_data.RootClass.CanIChase())
        {
            CancelPatrolling();
            _stateMachineHandler.ChangeState(new ChaseState(_stateMachineHandler, _data));
        }
    }

    public void OnExit()
    {
        CancelPatrolling();
        _data.AnimatorComponent.SetBool("Walk", false);
        _agent.isStopped = true; // Stop NavMeshAgent movement
        Debug.Log("Player exiting Patrolling State.");
    }

    private async UniTaskVoid Patrol(CancellationToken token)
    {
        // Enqueue all patrol points if the queue is empty
        if (_points.Count == 0)
        {
            foreach (Transform item in _data.RootClass.Points)
            {
                _points.Enqueue(item);
            }
        }

        while (_isPatrolling && _points.Count > 0)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("Patrolling Cancelled");
                return;
            }

            Transform nextPoint = _points.Dequeue();
            if (!_agent.isOnNavMesh)
            {
                Debug.LogError("NavMeshAgent is no longer on the NavMesh. Aborting patrol.");
                return;
            }

            _agent.SetDestination(nextPoint.position);

            // Wait for the agent to reach the point
            while (!_agent.pathPending && _agent.remainingDistance > _agent.stoppingDistance)
            {
                await UniTask.Yield();
            }

            _data.AnimatorComponent.SetBool("Walk", false);
            // Wait for 2 seconds at the point
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
            Debug.Log($"Patrolled to point: {nextPoint.name}");
            _data.AnimatorComponent.SetBool("Walk", true);

            // Re-enqueue the point to keep patrolling indefinitely
            _points.Enqueue(nextPoint);
        }
    }

    private void CancelPatrolling()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        _isPatrolling = false;
    }
}
