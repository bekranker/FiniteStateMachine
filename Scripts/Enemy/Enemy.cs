using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour, IBaseData
{

    #region Props
    [Header("---Components")]
    [Space(10)]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _player;
    public List<Transform> Points;
    [SerializeField] private TMP_Text _statusText;
    [Header("---Props")]
    [Space(10)]
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _minMaxChaseRange;
    #endregion


    private StateMachine<EnemyStateData<Enemy>> _stateMachine;
    private EnemyStateData<Enemy> _enemyStateData;



    void Start()
    {
        //State machine'i oluşturuyoruz;
        _stateMachine = new();
        //enemy data'yı atıyoruz (NOTE: burada ki data tamamen örnek amaçlıdır Scriptable Object kullanımı daha optimize olacaktır);
        _enemyStateData = new("Enemy", _animator, this, _statusText);

        //başlangıçta Idle State'e geçiriyoruz;
        _stateMachine.AddState(new IdleState(_stateMachine, _enemyStateData));
    }

    void Update()
    {
        //aktif olan state'i her frame de çağıran update metodu burası.
        _stateMachine.UpdateStates();
    }

    //bu fonksiyonu buraya yazmamın sebebi (state'lere teker teker yazmak yerine) buraya bir defa yazıp state'lerden çağırabilirim (her state'de kontrol etmem gerekcekti);
    public bool CanIChase()
    {
        //player ile düşman arasında ki mesafe
        float distance = Vector2.Distance(transform.localPosition, _player.localPosition);

        //artık düşman dibine kadar gelmiş oldu;
        if (distance <= _minMaxChaseRange.y)
        {
            return false;
        }
        //düşman takip edebilecek bir mesafede;
        else if (distance <= _minMaxChaseRange.x)
        {
            //takip edebilir;
            return true;
        }
        return false;
    }
}

/* Şimdi burada bir tık işler karışmış gibi gözüküyor ama aslında gayet açık ve kolay. T olarak herhangi bir IBaseData'yı miras alan classı ya da interface'i RootClass olarak atayabiliriz.
Bununla beraber istediğimiz bir class'a ya da interface'e ulaşabiliriz. Bunu da EnemyStatedata oluştururken atayarak yapıyoruz. Örneğin 

```
    public EnemyStateData(string name, Animator animatorComponent, T rootClass)
    {
        Name = name;
        AnimatorComponent = animatorComponent;
        RootClass = rootClass;   ====> burası Monobehaviour'a (Enemy sınıfına) ulaştığımız kısım.
    }
```
*/

// NOTE: Düşman için örnek bir data tipidir, bunun yerine Scriptable Object kullanımı daha doğru olur.
public class EnemyStateData<T> : IBaseData
{
    public string Name { get; set; }
    public Animator AnimatorComponent { get; set; }
    public T RootClass;
    public TMP_Text StatusText;

    public EnemyStateData(string name, Animator animatorComponent, T rootClass, TMP_Text statusText)
    {
        Name = name;
        AnimatorComponent = animatorComponent;
        RootClass = rootClass;
        StatusText = statusText;
    }
}