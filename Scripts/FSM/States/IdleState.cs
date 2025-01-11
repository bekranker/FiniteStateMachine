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


    //Idle State'de durmmamız gerektiği için isStopped = true olarka değiştirdik;
    public async void OnEnter()
    {
        Debug.Log("Enemy entered Idle State.");
        _data.StatusText.text = $"{_data.Name} - State: Idle";
        _data.NavMeshAgent.isStopped = true;
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

    //2 saniye bekliyoruz, eğer ki oyuncu gözükmediyse Patrolling State'e geçiyor;
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
        // eğer oyuncuyu görürsek anında takip moduna geçiyoruz;
        if (_data.RootClass.CanIChase())
        {
            CancelTask();
            _stateMachineHandler.ChangeState(new ChaseState(_stateMachineHandler, _data));
        }
    }

    //çıkış yaparken durmadığımızdan emin oluyoruz;
    public void OnExit()
    {
        Debug.Log("Player exiting Idle State.");
        _data.NavMeshAgent.isStopped = false;
        CancelTask();
    }
    //oyuncuyu görürsek takipe geçmeden önce DelayCall fonksiyonunu Kill etmek için token ile UniTask'i Cancellıyoruz;
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
