using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks; // Ensure UniTask is installed
using UnityEngine;
using System.Threading;

public class PatrollingState : IState<EnemyStateData<Enemy>>
{
    private IStateMachine<EnemyStateData<Enemy>> _stateMachineHandler;
    private EnemyStateData<Enemy> _data;

    private Queue<Transform> _points = new();
    private bool _isPatrolling = false;
    private CancellationTokenSource _cancellationTokenSource;

    // Constructor
    public PatrollingState(IStateMachine<EnemyStateData<Enemy>> stateMachine, EnemyStateData<Enemy> data)
    {
        _stateMachineHandler = stateMachine;
        _data = data;
    }

    public void OnEnter()
    {
        //düşman bakelenen alanda mı kontrol ediyoruz, değilse reutn ediyoruz (hataları önlemek için ama isterseniz kaldırabilirsiniz) ;
        if (!_data.NavMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a valid NavMesh. Patrolling cannot start.");
            return;
        }
        //Debug amaçlı düşman üzerinde ki yazıyı güncellediğim yer;
        _data.StatusText.text = $"{_data.Name} - State: Patrolling";
        //Walk animasyonunu başlatıyoruz (Loopta olduğu için kapatana dek çalışacaktır);
        _data.AnimatorComponent.SetBool("Walk", true);

        //UniTask'i başka state'e geçerken iptal edebilmek için bir CancellationToken oluşturuyorum;
        _cancellationTokenSource = new CancellationTokenSource();

        // Start patrolling
        if (!_isPatrolling)
        {
            _isPatrolling = true;
            Patrol(_cancellationTokenSource.Token).Forget();
        }

        Debug.Log("Player entered Patrolling State.");
    }

    public void OnUpdate()
    {
        // eğer oyuncu görüş alanında ise kovalayacak olan state'e geçiyor.
        if (_data.RootClass.CanIChase())
        {
            CancelPatrolling();
            _stateMachineHandler.ChangeState(new ChaseState(_stateMachineHandler, _data));
        }
    }

    //bu state'den çıkarken animasyonu kapatıyoruz, ayrıca durmadığından emin olmak için navmesh isStopped = false diyoruz;
    //UniTask'i iptal edip state'den çıkıyoruz;
    public void OnExit()
    {
        CancelPatrolling();
        _data.AnimatorComponent.SetBool("Walk", false);
        _data.NavMeshAgent.isStopped = false; // Stop NavMeshAgent movement
        Debug.Log("Player exiting Patrolling State.");
    }

    //Uni Task yerine IEnumerator kullanılabilirdi fakat UniTask dah optimizde ve Monobehaviour miras almama gerek kalmıyor(yada RootClass'a ekstra ulaşmama gerek kalmıyor) ;
    private async UniTaskVoid Patrol(CancellationToken token)
    {
        //eğer ki gidilecek başka point yoksa _data.RootClass'dan pointleri alıp tekrardan sıraya koyuyoruz;
        if (_points.Count == 0)
        {
            foreach (Transform item in _data.RootClass.Points)
            {
                _points.Enqueue(item);
            }
        }

        //gidilecek başka point yoksa hata almamak için break olacak;
        while (_points.Count > 0)
        {
            //eğer unitask iptal edilirse burası tetiklenecek
            if (token.IsCancellationRequested)
            {
                Debug.Log("Patrolling Cancelled");
                return;
            }

            Transform nextPoint = _points.Dequeue();
            _data.RootClass.Go(nextPoint.position);

            // Yürümeye başladığında animasyonu aç;
            _data.AnimatorComponent.SetBool("Walk", true);


            // düşmanın hedefe ulaşmasını bekliyoruz;
            while (_data.NavMeshAgent.pathPending || _data.NavMeshAgent.remainingDistance > _data.NavMeshAgent.stoppingDistance)
            {
                await UniTask.Yield();
            }

            // Noktaya ulaşıldığında animasyonu kapat;
            await UniTask.NextFrame();
            _data.AnimatorComponent.SetBool("Walk", false);
            // 2 saniye düşman nefesleniyor
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

            Debug.Log($"Patrolled to point: {nextPoint.name}");


            // az önce vardığımız pointi sıraya tekrar ekliyoruz ki loopa girsin;
            _points.Enqueue(nextPoint);
        }
    }

    //Uni Task iptal ettiğimiz kısım burası;    
    private void CancelPatrolling()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        _isPatrolling = false;
    }
}
