using UnityEngine;

public abstract class EnemyBaseState
{
    protected EnemyStateManager enemy;
    public abstract void EnterState(EnemyStateManager enemy);
    public abstract void UpdateState();
    public abstract void ExitState();
}
