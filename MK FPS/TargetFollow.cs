using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.position = target.position;
    }
}
