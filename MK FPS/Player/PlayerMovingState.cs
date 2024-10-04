using UnityEngine;
using static UnityEngine.InputSystem.Controls.AxisControl;
/// <summary>
/// Character movement on <c>FLAT TERRAIN</c>
/// </summary>
public class PlayerMovingState : PlayerStateBase
{
    private bool canSprint;
    public override void EnterState(PlayerStateManager player)
    {
        canSprint = player.staminaCurrentAmount > player.staminaMinAmountToStartSprint;
        player.interaction.StartFinding();
    }
    public override void UpdateState(PlayerStateManager player)
    {
        #region MOVEMENT
        float moveSpeed = player.speed;

        if (player.input.sprint && canSprint && player.input.move.magnitude > 0.1f)
        {
            player.input.crouch = false;
            moveSpeed *= player.sprintMult;
            player.staminaCurrentAmount -= player.staminaSprintDepletionRate * Time.deltaTime;
            player.UseStamina.Invoke();
        }
        else
        {
            player.staminaCurrentAmount += player.staminaNormalRecoveryRate * Time.deltaTime;
            canSprint = player.staminaCurrentAmount > player.staminaMinAmountToStartSprint;
        }
        player.staminaCurrentAmount = Mathf.Clamp(player.staminaCurrentAmount, 0f, player.staminaMaxAmount);
        if (player.staminaCurrentAmount == 0f) canSprint = false;

        if (player.input.crouch) moveSpeed *= player.crouchMult;
        
        Vector3 moveDirection = player.transform.right * player.input.move.x + player.transform.forward * player.input.move.y;
        moveDirection = moveDirection.normalized;

        if (player.cc.isGrounded)
        {
            player.animator.SetBool(player.animIDJump, false);
            player.animator.SetBool(player.animIDFreeFall, false);

            player.verticalSpeed = 0f;
            if (player.input.jump && player.staminaCurrentAmount > player.staminaJumpCost)
            {
                player.staminaCurrentAmount -= player.staminaJumpCost;
                player.UseStamina.Invoke();
                // the square root of jumpHeight * 2 * gravity * -1 (for negative gravity) =
                // how much velocity needed to reach desired jump height
                player.verticalSpeed = (float)Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
                player.animator.SetBool(player.animIDJump, true);
            }
            player.input.jump = false;
        }
        else
        {
            player.input.jump = false;
            player.animator.SetBool(player.animIDFreeFall, true);
        }

        // Basic calculation (+/- 0.5% error)
        player.verticalSpeed += player.gravity * Time.deltaTime;
        player.verticalSpeed = Mathf.Clamp(player.verticalSpeed, -player.terminalVelocity, player.terminalVelocity);

        // PERFECT CALCULATION (+/- 0.05% error) part 1
        // https://youtu.be/yGhfUcPjXuE?si=LHiG6PZVTnYm798U&t=502 -> explanation
        // verticalSpeed += gravity * timeDelta * 0.5f;
        // verticalSpeed = Mathf.Clamp(verticalSpeed, -terminalVelocity, terminalVelocity);

        // Find the ANGLE of the current SLOPE, Switch to SlidingState if the angle is too big
        // must be before Move();
        if (player.cc.isGrounded && Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f, player.slopeCheckLayerMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle >= player.cc.slopeLimit)
            {
                player.SwitchState(player.SlidingState);
                player.input.crouch = false;
            }
        }

        player.cc.Move(moveDirection * moveSpeed * Time.deltaTime + new Vector3(0f, player.verticalSpeed, 0f) * Time.deltaTime);

        // PERFECT CALCULATION (+/- 0.05% error) part 2
        // verticalSpeed += gravity * timeDelta * 0.5f;
        // verticalSpeed = Mathf.Clamp(verticalSpeed, -terminalVelocity, terminalVelocity);

        #endregion

        if (player.climb)
        {
            player.climb = false;
            player.SwitchState(player.ClimbingState);
            player.input.crouch = false;
        }

        player.animationRunBlendCur = Mathf.Lerp(player.animationRunBlendCur, moveSpeed * moveDirection.magnitude, Time.deltaTime * player.animationRunBlendSpeed);
        player.animator.SetBool(player.animIDGrounded, player.cc.isGrounded);
        player.animator.SetFloat(player.animIDSpeed, player.animationRunBlendCur);
        player.animator.SetFloat(player.animIDMotionSpeed, player.input.move.normalized.magnitude);

        #region CAMERA
        // sensitivity gets multiplied by delta time for game pad to make it unbound to fps
        // mouse is inherently unbound to fps (it uses distance not -1 to 1)
        // MAKE SURE TO CREATE A SEPERATE STATE MACHINE FOR CAMERA
        float mult = (player.input.IsUsingKeyboardAndMouse) ? 1.0f : Time.deltaTime;

        player.cameraTargetPitch -= player.input.look.y * player.sensitivity * mult;
        player.cameraTargetPitch = CameraAngleClamp(player.cameraTargetPitch, player.downClamp, player.upClamp);
        player.targetCamera.transform.localRotation = Quaternion.Euler(player.cameraTargetPitch, 0f, 0f);
        player.transform.Rotate(Vector3.up * player.input.look.x * player.sensitivity * mult);

        #endregion
    }
    public override void ExitState(PlayerStateManager player)
    {
        player.interaction.StopFinding();
    }
    
    private static float CameraAngleClamp(float value, float min, float max)
    {
        if (value < -360f) value += 360f;
        if (value > 360f) value -= 360f;
        return Mathf.Clamp(value, min, max);
    }
}
