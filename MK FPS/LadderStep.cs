using UnityEngine;

public class LadderStep : MonoBehaviour
{
    public Transform lHand;
    public Transform rHand;
    public bool taken;

    private DrawEffector eff;

    private void Start()
    {
        TryGetComponent<DrawEffector>(out eff);
        if (eff) eff.hide = true;
    }

    private void Update()
    {
        if (eff != null) eff.hide = !taken;
    }


}
