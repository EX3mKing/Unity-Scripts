using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerRigManager : MonoBehaviour
{
    public Rig climbRig;
    private Coroutine climbCoroutine;
    private PlayerStateManager player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerStateManager>();
    }

    public void StartClimbing(IN_Ladders ladder)
    {
        climbRig.weight = 1.0f;
        climbCoroutine = StartCoroutine(FindHandTargetsOnLadder(ladder));
    }

    public void StopClimbing()
    {
        StopCoroutine(climbCoroutine);
        climbRig.weight = 0f;
    }

    IEnumerator FindHandTargetsOnLadder(IN_Ladders ladder)
    {
        //Debug.Log("Start Finding Handholds");
        while (true)
        {
            foreach (var step in ladder.steps)
            {
                if (step.taken) continue;

                // distance from player.y
                float ld = Mathf.Abs(Mathf.Abs(player.climbLHandTarget.position.y) - Mathf.Abs(player.targetCamera.transform.position.y));
                float rd = Mathf.Abs(Mathf.Abs(player.climbRHandTarget.position.y) - Mathf.Abs(player.targetCamera.transform.position.y));
                float sd = Mathf.Abs(Mathf.Abs(step.transform.position.y) - Mathf.Abs(player.targetCamera.transform.position.y));
                //print(ld + ", " + rd + ", " + sd);
                if (ld > rd)
                {
                    if (sd < ld)
                    {
                        if (ladder.heightsteps.ContainsKey(Mathf.Round(player.climbLHandTarget.position.y * 100.0f) * 0.01f))
                        {
                            ladder.heightsteps[Mathf.Round(player.climbLHandTarget.position.y * 100.0f) * 0.01f].taken = false;
                            //print("realease L");
                        }
                        //print("take L");
                        player.climbLHandTarget.position = step.lHand.position;
                        step.taken = true;
                    }
                }
                else
                {
                    if (sd < rd)
                    {
                        if (ladder.heightsteps.ContainsKey(Mathf.Round(player.climbRHandTarget.position.y * 100.0f) * 0.01f))
                        {
                            ladder.heightsteps[Mathf.Round(player.climbRHandTarget.position.y * 100.0f) * 0.01f].taken = false;
                            //print("release R");
                        }
                        player.climbRHandTarget.position = step.rHand.position;
                        //print("take R");
                        step.taken = true;
                    }
                }
            }

            yield return new WaitForSeconds(player.timeBetweenHandPlacementUpdates);
        }
    }
}
