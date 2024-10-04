using UnityEngine;

public class EnemyReturnState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager esm)
    {
        //Debug.Log("Enter Return");
        enemy = esm;
    }
    public override void UpdateState()
    {
        //Debug.Log("Update Return");
        enemy.navigation.destination = enemy.startingPosition;
        if (Vector3.Distance(enemy.transform.position, enemy.startingPosition) < 0.2f) enemy.SwitchState(enemy.PassiveState);
        if (enemy.playerInStopAggroArea) enemy.SwitchState(enemy.AggroState);
    }
    public override void ExitState()
    {
        //Debug.Log("Exit Return");
    }
}
