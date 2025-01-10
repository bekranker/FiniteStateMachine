using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class IdleState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;


    //Constructor
    public IdleState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
    }

    public async void OnEnter()
    {
        Debug.Log("Enemy entered Idle State.");
        _data.StatusText.text = $"{_data.Name} - State: Idle";
        await delayedCall();
    }
    private async UniTask delayedCall()
    {
        //2 saniye bekleyip chasse'e geçecek
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _stateMachineHandler.AddState(new PatrollingState(_stateMachineHandler, _data));
    }
    public void OnUpdate()
    {
        //eğer ki takip mesafesinde ise takip etme state'ine geçecek
        if (_data.RootClass.CanIChase())
        {
            _stateMachineHandler.AddState(new ChaseState(_stateMachineHandler, _data));
        }
    }

    public void OnExit()
    {
        Debug.Log("Player exiting Idle State.");
    }
}