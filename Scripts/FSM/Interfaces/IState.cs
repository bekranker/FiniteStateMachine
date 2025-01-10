public interface IState<T> where T : IBaseData
{
    void Init(IStateMachine<T> stateMachineHandler, T data);
    void OnEnter();
    void OnExit();
    void OnUpdate();
}