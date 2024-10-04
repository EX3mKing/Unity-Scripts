using UnityEngine;

public class HideRenderrer : MonoBehaviour
{
    private Renderer renderrer;
    private void Awake()
    {
        TryGetComponent<Renderer>(out renderrer);
        if (renderrer != null) renderrer.enabled = false;
    }
}
