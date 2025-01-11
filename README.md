# 🎮 Unity Finite State Machine (FSM) for Enemy AI

A Unity implementation of a Finite State Machine (FSM) system designed for enemy AI behavior control in games. This project includes a flexible state machine framework with multiple enemy states such as `Idle`, `Chase`, `Attack`, and `Patrolling`.

## 📦 Key Features

- ✅ **Modular Design:** Easily extendable state system using interfaces.
- ✅ **Multiple Enemy States:** Includes Idle, Chase, Attack, and Patrolling behaviors.
- ✅ **AI Behavior Simulation:** Simulates complex enemy decision-making using C# generics and Unity components.
- ✅ **Flexible Integration:** Can be adapted to various game projects and enemy types.

---

## 📖 Project Structure

- **Interfaces:**

  - `IBaseData` → Base interface for enemy state data.
  - `IState<T>` → Defines methods for individual states (`OnEnter`, `OnUpdate`, `OnExit`).
  - `IStateMachine<T>` → Manages state transitions and updates.

- **State Implementations:**

  - `IdleState`
  - `AttackState`
  - `ChaseState`
  - `PatrollingState`

- **Core State Machine:**

  - `StateMachine<T>` → Handles state transitions and updates.

- **Enemy and Data Classes:**
  - `Enemy` → Main MonoBehaviour for enemy logic.
  - `EnemyStateData<T>` → Data class holding enemy references and stats.

---

## 🛠️ Setup and Usage

1. **Import the Scripts:** Add the provided scripts to your Unity project.
2. **Create an Enemy:** Attach the `Enemy` script to an enemy GameObject.
3. **Assign Components:** Assign `NavMeshAgent`, `Animator`, `TextMeshPro` components in the Inspector.
4. **Configure Waypoints:** Add waypoints for patrolling behavior in the `Points` list.
5. **Play:** Press Play to see the FSM in action!

---

## 🚀 Code Example

```csharp
void Start()
{
    _stateMachine = new StateMachine<EnemyStateData<Enemy>>();
    _enemyStateData = new EnemyStateData<Enemy>("Enemy", _animator, this, _statusText, _navMeshAgent, _player);
    _stateMachine.ChangeState(new IdleState(_stateMachine, _enemyStateData));
}
```

```markdown
## 📂 How the FSM Works

1. **State Transitions:** Each state (`IdleState`, `AttackState`, etc.) implements the `IState<T>` interface and defines behavior for `OnEnter()`, `OnUpdate()`, and `OnExit()`.
2. **State Changes:** The `StateMachine<T>` class handles transitions via the `ChangeState()` method.
3. **State Execution:** The active state is updated every frame using `UpdateStates()`.

---

## 🧩 Extending the System

### Creating a New State:

1. Create a new class implementing `IState<T>`.
2. Define the behavior for `OnEnter()`, `OnUpdate()`, and `OnExit()`.
3. Add state transitions based on conditions.

### Adding Custom Data:

- Modify `EnemyStateData<T>` to include custom properties needed for new behaviors.

---

## 📃 License

This project is licensed under the MIT License - feel free to modify and use it for your own projects.
```
