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
        _data.AnimatorComponent.SetTrigger("Run");
        _data.StatusText.text = "State: Chase";

        Debug.Log("Player entered Chase State.");
    }

    public void OnUpdate()
    {
        //eğer ki takip mesafesinde ise takip etme state'ine geçecek
        if (!_data.RootClass.CanIChase())
        {
            _stateMachineHandler.AddState(new IdleState(_stateMachineHandler, _data));
        }
    }

    public void OnExit()
    {
        Debug.Log("Player exiting Chase State.");
    }
}