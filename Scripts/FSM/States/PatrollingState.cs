using System.Collections.Generic;
using Cysharp.Threading.Tasks; // Ensure UniTask is installed
using UnityEngine;
using UnityEngine.AI;

public class PatrollingState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;

    private Queue<Transform> _points = new();
    private NavMeshAgent _agent;
    private bool _isMoving = false;

    // Constructor
    public PatrollingState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
        _agent = _data.NavMeshAgent; // Assuming Enemy has a NavMeshAgent
    }

    public void OnEnter()
    {
        _data.AnimatorComponent.SetBool("Walk", true);
        _data.StatusText.text = $"{_data.Name} - State: Patrolling";

        // Enqueue all patrol points
        foreach (Transform item in _data.RootClass.Points)
        {
            _points.Enqueue(item);
        }

        // Start patrolling
        Patrol().Forget();
        Debug.Log("Player entered Patrolling State.");
    }

    public void OnUpdate()
    {
        // Switch to ChaseState if the enemy can chase
        if (_data.RootClass.CanIChase())
        {
            _stateMachineHandler.AddState(new ChaseState(_stateMachineHandler, _data));
        }
    }

    public void OnExit()
    {
        _data.AnimatorComponent.SetBool("Walk", false);
        _agent.isStopped = true; // Stop NavMeshAgent movement
        Debug.Log("Player exiting Patrolling State.");
    }

    private async UniTaskVoid Patrol()
    {
        while (_points.Count > 0)
        {
            Transform nextPoint = _points.Dequeue();
            _agent.SetDestination(nextPoint.position);

            // Wait for the agent to reach the point
            while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
            {
                await UniTask.Yield();
            }

            // Wait for 1 second at the point
            await UniTask.Delay(1000);

            Debug.Log($"Patrolled to point: {nextPoint.name}");
        }

        Debug.Log("Finished patrolling all points.");
    }
}
