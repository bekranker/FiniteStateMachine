using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class IdleState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;
    private CancellationTokenSource _cancellationTokenSource;

    // Constructor
    public IdleState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
    }

    public async void OnEnter()
    {
        Debug.Log("Enemy entered Idle State.");
        _data.StatusText.text = $"{_data.Name} - State: Idle";

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await delayedCall(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Idle State task was canceled.");
        }
    }

    private async UniTask delayedCall(CancellationToken token)
    {
        // Wait for 2 seconds or cancel
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
        if (!token.IsCancellationRequested)
        {
            _stateMachineHandler.ChangeState(new PatrollingState(_stateMachineHandler, _data));
        }
    }

    public void OnUpdate()
    {
        // Transition to ChaseState if the enemy can chase
        if (_data.RootClass.CanIChase())
        {
            CancelTask();
            _stateMachineHandler.ChangeState(new ChaseState(_stateMachineHandler, _data));
        }
    }

    public void OnExit()
    {
        Debug.Log("Player exiting Idle State.");
        CancelTask();
    }

    private void CancelTask()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
