using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    public LayerMask interactionLayerMask;
    private InputManager im;
    public float timeBetweenUpdates;
    public float reach;
    [HideInInspector]
    public Interactable interactable;
    private Coroutine findingCoroutine;
    public bool automaticStart;
    private void Start()
    {
        im = FindFirstObjectByType<InputManager>();
        im.Interact.AddListener(Interact);
        if(automaticStart) findingCoroutine = StartCoroutine(FindInteractable());
    }

    private IEnumerator FindInteractable()
    {
        while (true)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, reach, interactionLayerMask))
            {
                hit.transform.TryGetComponent<Interactable>(out Interactable temp);
                if (temp != null) interactable = temp;
            }
            else
            {
                interactable = null;
            }
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
    }

    private void Interact()
    {
        if(!interactable) return;
        interactable.Interact();
    }

    public void StopFinding()
    {
        StopCoroutine(findingCoroutine);
    }

    public void StartFinding()
    {
        if (findingCoroutine != null) StopCoroutine(findingCoroutine);
        findingCoroutine = StartCoroutine(FindInteractable());
    }
}
