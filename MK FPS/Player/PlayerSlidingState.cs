using UnityEngine;
/// <summary>
/// Player movement on <c>STEEP TERRAIN</c>
/// </summary>
public class PlayerSlidingState : PlayerStateBase
{
    private float timer;
    public override void EnterState(PlayerStateManager player)
    {
        player.animator.Rebind();
        player.animator.Update(0f);
        player.animationRunBlendCur = 0;
        player.animator.SetBool(player.animIDGrounded, true);
        player.animator.SetFloat(player.animIDSpeed, 0);
        player.animator.SetFloat(player.animIDMotionSpeed, 0);
        player.animator.SetBool(player.animIDJump, false);
        player.animator.SetBool(player.animIDFreeFall, false);
    }
    public override void UpdateState(PlayerStateManager player)
    {
        #region SLIDING
        player.verticalSpeed += player.gravity * Time.deltaTime;
        player.verticalSpeed = Mathf.Clamp(player.verticalSpeed, -player.terminalVelocity, player.terminalVelocity);

        if (Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f, player.slopeCheckLayerMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            Vector3 slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, player.verticalSpeed, 0), hit.normal);

            player.cc.Move(slopeSlideVelocity * Time.deltaTime * player.slideSpeed);

            if (angle >= player.cc.slopeLimit)
            {
                timer = player.timeToGetUpAfterSliding;
            }
        }

        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            player.SwitchState(player.MovingState);
        }

        #endregion

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
        //throw new System.NotImplementedException();
    }

    private static float CameraAngleClamp(float value, float min, float max)
    {
        if (value < -360f) value += 360f;
        if (value > 360f) value -= 360f;
        return Mathf.Clamp(value, min, max);
    }
}
