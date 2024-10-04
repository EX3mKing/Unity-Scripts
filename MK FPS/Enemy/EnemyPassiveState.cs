using UnityEngine;

public class EnemyPassiveState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager esm)
    {
        //Debug.Log("Enter Passive");
        enemy = esm;
        //if (enemy.enemyWakeUpType == EnemyWakeUpType.Threatening) enemy.StartAggroAreaEvent.entered.AddListener(StartAgro);
    }
    public override void UpdateState()
    {
        //Debug.Log("Update Passive");
        float angle = Vector3.Angle(enemy.player.transform.position, enemy.transform.position);
        
        if (enemy.playerInStartAggroArea && enemy.enemyWakeUpType == EnemyWakeUpType.Threatening)
        {
            if (enemy.playerSeen || !enemy.player.input.crouch) enemy.SwitchState(enemy.AggroState);
        }
        if (enemy.enemyWakeUpType == EnemyWakeUpType.Passive && enemy.player.input.interact && enemy.playerInStopAggroArea)
        {
            enemy.SwitchState(enemy.AggroState);
            enemy.player.input.interact = false;
        } 
    }
    public override void ExitState()
    {
        //Debug.Log("Exit Passive");
    }

    //private void StartAgro()
    //{
    //    enemy.SwitchState(enemy.AggroState);
    //    enemy.StartAggroAreaEvent.entered.RemoveListener(StartAgro);
    //}
}
