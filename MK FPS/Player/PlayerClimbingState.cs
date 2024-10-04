using System.Collections;
using UnityEngine;
/// <summary>
/// Player movement for climbing <c>LADDERS</c>
/// </summary>
public class PlayerClimbingState : PlayerStateBase
{
    private IN_Ladders ladder;
    private bool canSprint;
    private bool firemanSlide;

    public override void EnterState(PlayerStateManager player)
    {
        player.cc.enabled = false;
        player.interaction.interactable.TryGetComponent<IN_Ladders>(out ladder);

        if (player.transform.position.y - player.cc.height > ladder.startPointLower.position.y)
        {
            player.transform.position = ladder.startPointHigher.position;
            player.transform.rotation = ladder.startPointHigher.rotation;
        }
        else
        {
            player.transform.position = ladder.startPointLower.position;
            player.transform.rotation = ladder.startPointLower.rotation;
        }
        player.input.crouch = false;

        player.animator.Rebind();
        player.animator.Update(0f);
        player.animationRunBlendCur = 0;
        player.animator.SetBool(player.animIDGrounded, true);
        player.animator.SetFloat(player.animIDSpeed, 0);
        player.animator.SetFloat(player.animIDMotionSpeed, 0);
        player.animator.SetBool(player.animIDJump, false);
        player.animator.SetBool(player.animIDFreeFall, false);

        player.rm.StartClimbing(ladder);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        player.targetCamera.transform.rotation = Quaternion.identity;
        float speed = player.climbNormalSpeed;
        if (player.input.crouch)
        {
            firemanSlide = true;
            player.input.crouch = false;
        }

        if (firemanSlide)
        {
            speed *= player.firemanLadderSlideSpeedMult;
            player.transform.position += -Vector3.up * speed * Time.deltaTime;
        }
        else
        {
            if (player.input.sprint && canSprint && player.input.move.magnitude > 0.1f)
            {
                speed *= player.climbSprintSpeedMult;
                player.staminaCurrentAmount -= player.staminaFastClimbDepletionRate * Time.deltaTime;
                player.UseStamina.Invoke();
            }
            else
            {
                player.staminaCurrentAmount += player.staminaNormalRecoveryRate * Time.deltaTime;
                canSprint = player.staminaCurrentAmount > player.staminaMinAmountToStartSprint;
            }

            player.transform.position += Vector3.up * player.input.move.normalized.y * speed * Time.deltaTime;
        }

        player.staminaCurrentAmount = Mathf.Clamp(player.staminaCurrentAmount, 0f, player.staminaMaxAmount);
        if (player.staminaCurrentAmount == 0f) canSprint = false;

        if (player.transform.position.y > ladder.endPoint.position.y - player.cc.height)
        {
            player.transform.position = ladder.endPoint.position;
            player.SwitchState(player.MovingState);
        }
        
        if (player.transform.position.y < ladder.startPointLower.position.y - player.cc.height*0.1f)
        {
            player.transform.position = ladder.startPointLower.position;
            player.SwitchState(player.MovingState);
            firemanSlide = false;
        }

        player.climbLHandIK.position = Vector3.Lerp(player.climbLHandIK.position, player.climbLHandTarget.position, Time.deltaTime * player.climbHandInterpolationSpeed);
        player.climbRHandIK.position = Vector3.Lerp(player.climbRHandIK.position, player.climbRHandTarget.position, Time.deltaTime * player.climbHandInterpolationSpeed);
    }
    public override void ExitState(PlayerStateManager player)
    {
        player.cc.enabled = true;
        ladder.ResetTakenStairs();
        player.rm.StopClimbing();
    }
}
