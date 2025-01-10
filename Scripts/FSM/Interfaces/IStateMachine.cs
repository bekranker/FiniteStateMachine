//bu interface'i miras alan sınıflar State Machine Handler görevi görecektir, miras alan sınıf herhangi bir objede bulunursa o objede FSM bulunmuş olacaktır.
public interface IStateMachine<T> where T : IBaseData
{
    //her frame'de child statelerden en üsttekini (i.e aktif olan state'i) execute edebilmemiz için gerekli
    void UpdateStates();
    //Stack'e state eklemek için
    void ChangeState(IState<T> state);
}