using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerEvent : MonoBehaviour
{
    public UnityEvent entered;
    public UnityEvent exited;

    private void OnTriggerEnter(Collider other)
    {
        entered.Invoke();
    }
    private void OnTriggerExit(Collider other) 
    {
        exited.Invoke();
    }
}
