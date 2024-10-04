using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyAggroState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager esm)
    {
        enemy = esm;
        //Debug.Log("Enter Aggro");
        //if (enemy.enemyFollowType == EnemyFollowType.Zone) enemy.StopAggroAreaEvent.exited.AddListener(EndAgro);
    }
    public override void UpdateState()
    {
        //Debug.Log("Update Aggro");
        if (enemy.playerInStopAggroArea) enemy.navigation.destination = enemy.player.transform.position;
        else enemy.SwitchState(enemy.ReturnState);
    }
    public override void ExitState()
    {
        //Debug.Log("Exit Aggro");
    }

    //private void EndAgro()
    //{
    //    enemy.SwitchState(enemy.ReturnState);
    //    enemy.StopAggroAreaEvent.exited.RemoveListener(EndAgro);
    //}
}
