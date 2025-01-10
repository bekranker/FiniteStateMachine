using System.Collections.Generic;


/*bu sınıf FSM'i kontrol eden sınıf;
* eğer koşul doğru olursa (herhangi bir state'de iken) o state buradan yeni state'e geçmek için AddState'i çağırıp yeni state'in execute edilmesini sağlayabilir;
*/
public class StateMachine<T> : IStateMachine<T> where T : IBaseData
{
    //Stateler burada birikiyor.
    private IState<T> _state;
    //herhangi bir state var mı yok mu ona bakıyor varsa true yoksa false döndürür.
    private bool HasAnyState => _state != null;

    //State eklediğimiz kısım. İlk başta anlık state varsa (stack'e Peek ile anlık execute'lanan state olup olmadığına bakıyoruz) Exit ediyoruz. 
    //Daha sonra eklerken state'i initialize ederiz daha sonra yeni state'i Enter edip OnUpdate'e geçmesi için Stack'e pushluyoruz.
    public void AddState(IState<T> state)
    {
        if (HasAnyState)
            _state.OnExit();

        state.OnEnter();
        _state = state;
    }
    //Stack'de bulunan en üstteki state'i her frame çağırıyoruz. Stack'den çıkarmamak için Peek() fonksiyonu ile stack item'a bakıyoruz.
    public void UpdateStates()
    {
        if (!HasAnyState)
            return;

        _state.OnUpdate();
    }
}