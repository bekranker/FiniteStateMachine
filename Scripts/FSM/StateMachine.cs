using System.Collections.Generic;


/*bu sınıf FSM'i kontrol eden sınıf;
* eğer koşul doğru olursa (herhangi bir state'de iken) o state buradan yeni state'e geçmek için AddState'i çağırıp yeni state'in execute edilmesini sağlayabilir;
*/
public class StateMachineHandler<T> : IStateMachine<T> where T : IBaseData
{
    //Stateler burada birikiyor.
    private readonly Stack<IState<T>> _states = new();
    //herhangi bir state var mı yok mu ona bakıyor varsa true yoksa false döndürür.
    private bool HasAnyState => _states.Count > 0;


    //State eklediğimiz kısım. İlk başta anlık state varsa (stack'e Peek ile anlık execute'lanan state olup olmadığına bakıyoruz) Exit ediyoruz. 
    //Daha sonra eklerken state'i initialize ederiz daha sonra yeni state'i Enter edip OnUpdate'e geçmesi için Stack'e pushluyoruz.
    public void AddState(IState<T> state, T stateData)
    {
        if (HasAnyState)
            _states.Peek().OnExit();

        state.Init(this, stateData);
        state.OnEnter();
        _states.Push(state);
    }
    //Herhangi bir state olup olmadığına bakıp varsa en üsttekini çıkarıyoruz ve bir altındaki state'e (eğer ki stack'de varsa) OnEnter ile geçiş yapıyoruz. Daha sonra OnUpdate metodu sürekli çalışıyor.
    public void RemoveState()
    {
        if (!HasAnyState)
            return;

        _states.Pop().OnExit();
        if (HasAnyState)
            _states.Peek().OnEnter();
    }

    //Stack'de bulunan en üstteki state'i her frame çağırıyoruz. Stack'den çıkarmamak için Peek() fonksiyonu ile stack item'a bakıyoruz.
    public void UpdateStates()
    {
        if (!HasAnyState)
            return;

        _states.Peek().OnUpdate();
    }
}