using System.Collections;
using UnityEngine;

public class PatrollingState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;


    //Constructor
    public PatrollingState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
    }

    public void OnEnter()
    {
        _data.AnimatorComponent.SetTrigger("Walk");
        _data.StatusText.text = "State: Patrolling";

        Debug.Log("Player entered Patrolling State.");
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
        Debug.Log("Player exiting Patrolling State.");
    }
}