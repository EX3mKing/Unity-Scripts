using UnityEngine;

public abstract class PlayerStateBase
{
    public abstract void EnterState(PlayerStateManager state);
    public abstract void UpdateState(PlayerStateManager state);
    public abstract void ExitState(PlayerStateManager state);
}
