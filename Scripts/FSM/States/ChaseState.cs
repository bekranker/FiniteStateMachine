using System.Collections;
using UnityEngine;

public class ChaseState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;

    //Constructor
    public ChaseState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
    }

    //koşu animasyonunu tetikliyoruz
    public void OnEnter()
    {
        _data.AnimatorComponent.SetBool("Run", true);
        _data.StatusText.text = $"{_data.Name} - State: Chase";
        Debug.Log("Player entered Chase State.");
        _data.NavMeshAgent.SetDestination(_data.Player.position);
    }

    public void OnUpdate()
    {
        _data.NavMeshAgent.SetDestination(_data.Player.position);

        //eğer ki takip mesafesinde ise takip etme state'ine geçecek
        if (_data.RootClass.IsTooCloseToMe())
        {
            _stateMachineHandler.AddState(new AttackState(_stateMachineHandler, _data));
        }
        else if (!_data.RootClass.CanIChase())
        {
            _stateMachineHandler.AddState(new IdleState(_stateMachineHandler, _data));
        }
    }

    public void OnExit()
    {
        _data.AnimatorComponent.SetBool("Run", false);

        Debug.Log("Player exiting Chase State.");
    }
}