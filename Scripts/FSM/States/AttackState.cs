using System.Collections;
using UnityEngine;

public class AttackState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;


    //Constructor
    public AttackState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
    }

    public void OnEnter()
    {
        _data.AnimatorComponent.SetBool("Attack", true);
        _data.StatusText.text = $"{_data.Name} - State: Patrolling";



        Debug.Log("Player entered Patrolling State.");
    }

    public void OnUpdate()
    {
        if (!_data.RootClass.IsTooCloseToMe() && _data.RootClass.CanIChase())
        {
            _stateMachineHandler.AddState(new ChaseState(_stateMachineHandler, _data));
        }
        else if (!_data.RootClass.CanIChase())
        {
            _stateMachineHandler.AddState(new IdleState(_stateMachineHandler, _data));
        }
        Debug.Log("Enemy is Attacking");
    }

    public void OnExit()
    {
        _data.AnimatorComponent.SetBool("Attack", false);
        Debug.Log("Player exiting Patrolling State.");
    }
}