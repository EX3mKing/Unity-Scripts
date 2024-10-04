using System.Collections;
using UnityEngine;

public class HandPlacement : MonoBehaviour
{
    public Transform player;
    public float speed;
    public Transform l;
    public Transform r;
    public Transform targetL;
    public Transform targetR;
    public IN_Ladders ladder;
    public float wait;

    private void Start()
    {
        targetL.position = l.position;
        targetR.position = l.position;
        StartCoroutine(FindHandTargets());
    }

    private void Update()
    {
        l.position = Vector3.Lerp(l.position, targetL.position, Time.deltaTime * speed);
        r.position = Vector3.Lerp(r.position, targetR.position, Time.deltaTime * speed);
    }

    IEnumerator FindHandTargets()
    {
        while (true)
        {
            foreach (var step in ladder.steps)
            {
                if (step.taken) continue;

                // distance from player.y
                float ld = Mathf.Abs(Mathf.Abs(targetL.transform.position.y) - Mathf.Abs(player.transform.position.y));
                float rd = Mathf.Abs(Mathf.Abs(targetR.transform.position.y) - Mathf.Abs(player.transform.position.y));
                float sd = Mathf.Abs(Mathf.Abs(step.transform.position.y) - Mathf.Abs(player.transform.position.y));

                if (ld > rd)
                {
                    if (sd < ld)
                    {
                        if (ladder.heightsteps.ContainsKey(targetL.position.y))
                        {
                            ladder.heightsteps[targetL.position.y].taken = false;
                            print("releaseL " + step.name);
                        }
                        targetL.position = step.lHand.position;
                        step.taken = true;
                        print("takeL " + step.name);
                    }
                }
                else
                {
                    if (sd < rd)
                    {
                        if (ladder.heightsteps.ContainsKey(targetR.position.y))
                        {
                            ladder.heightsteps[targetR.position.y].taken = false;
                            print("releaseR " + step.name);
                        }
                        targetR.position = step.rHand.position;
                        print("takeR " + step.name);
                        step.taken = true;
                    }
                }
            }

            yield return new WaitForSeconds(wait);
        }
    }
}
