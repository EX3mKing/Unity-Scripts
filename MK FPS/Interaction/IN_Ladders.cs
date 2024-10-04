using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IN_Ladders : Interactable
{
    public Transform startPointLower;
    public Transform startPointHigher;
    public Transform endPoint;

    public List<LadderStep> steps;
    public float handToStepSpeed;

    [Header("Hand Placement")]
    public Mesh mesh;
    public float size;
    public Color color;

    public Dictionary<float, LadderStep> heightsteps = new Dictionary<float, LadderStep>();

    private void Start()
    {
        base.Start();
        foreach (var step in steps)
        {
            heightsteps.Add(Mathf.Round(step.transform.position.y * 100.0f) * 0.01f, step);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = color;
        foreach (var step in steps)
        {
            Gizmos.DrawMesh(mesh, step.lHand.position, Quaternion.identity, Vector3.one * size);
            Gizmos.DrawMesh(mesh, step.rHand.position, Quaternion.identity, Vector3.one * size);
        }
    }

    public override void Interact()
    {
        player.climb = true;
    }

    public void ResetTakenStairs()
    {
        foreach (var step in steps)
        {
            step.taken = false;
        }
    }
}
