using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    protected PlayerStateManager player;
    public virtual void Start()
    {
        player = FindFirstObjectByType<PlayerStateManager>();
    }
    public virtual void Interact()
    {

    }
}
