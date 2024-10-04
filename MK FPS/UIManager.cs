using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStateManager player;
    [Header("Stamina")]
    public CanvasGroup staminaUI;
    public Slider staminaBar;
    public Slider staminaUsageBar;
    [Tooltip("After using stamina, how long does it take for the stamina bar to pop up")]
    public float timeToShowStaminaBar = 0.1f;
    [Tooltip("After waiting for stamina bar to fill how long does it take to START HIDING the bar")]
    public float timeBeforeHidingStaminaBar = 0.1f;
    [Tooltip("How long does the HIDING take")]
    public float timeToHideStaminaBar = 0.25f;
    public float staminaUsageFollowSpeed = 1.1f;
    public float staminaBarFillUpAlphaTarget = 0.3f;
    [Tooltip("Lerp speed for stamina bar alpha when waiting to fill up")]
    public float staminaBarFillUpAlphaSpeed = 1.0f;

    private Coroutine staminaShowHideCoroutine;
    private Coroutine staminaBarUpdateCoroutine;

    private bool isStaminaUsed = false;
    private bool isStaminaShowing = false;
    private Coroutine staminaShowCoroutine;
    private Coroutine staminaHideCoroutine;
    private Coroutine staminaFillUpCoroutine;
    private void Start()
    {
        staminaBar.value = player.staminaCurrentAmount / player.staminaMaxAmount;
        staminaUsageBar.value = staminaBar.value;
        player.UseStamina.AddListener(ShowStaminaBar);
    }

    public void ShowStaminaBar()
    {
        if (staminaHideCoroutine != null) StopCoroutine(staminaHideCoroutine);
        if (staminaFillUpCoroutine != null) StopCoroutine(staminaFillUpCoroutine);
        if (!isStaminaUsed && !isStaminaShowing) staminaShowCoroutine = StartCoroutine(ShowingStaminaBar());
        isStaminaUsed = true;
    }

    IEnumerator ShowingStaminaBar()
    {
        if(staminaBarUpdateCoroutine != null) StopCoroutine(staminaBarUpdateCoroutine);
        staminaBarUpdateCoroutine = StartCoroutine(UpdateStaminaBars());

        float a = staminaUI.alpha;
        do
        {
            isStaminaShowing = true;
            // Same as using lerp
            a += Time.deltaTime / timeToShowStaminaBar;
            a = Mathf.Clamp01(a);
            staminaUI.alpha = a;
            isStaminaUsed = false;
            yield return null;
        }
        while (a < 1f || isStaminaUsed);
        staminaFillUpCoroutine = StartCoroutine(StaminaFillUpBar());
        isStaminaShowing = false;
        
    }

    // Coroutine used when waiting for the stamina bar to fill up before hiding it
    IEnumerator StaminaFillUpBar()
    {
        float a = staminaUI.alpha;
        while (player.staminaCurrentAmount < player.staminaMaxAmount)
        {
            a = Mathf.Lerp(a, staminaBarFillUpAlphaTarget, Time.deltaTime * staminaBarFillUpAlphaSpeed);
            staminaUI.alpha = a;
            yield return null;
        }
        staminaHideCoroutine = StartCoroutine(HideStaminaBar());
    }

    IEnumerator HideStaminaBar()
    {
        yield return new WaitForSeconds(timeBeforeHidingStaminaBar);
        float a = staminaUI.alpha;
        do
        {
            // Same as using lerp
            a -= Time.deltaTime / timeToHideStaminaBar;
            a = Mathf.Clamp01(a);
            staminaUI.alpha = a;
            yield return null;
        }
        while (a > 0f);
        StopCoroutine(staminaBarUpdateCoroutine);
    }

    // Updates the stamina and usage bar values
    IEnumerator UpdateStaminaBars()
    {
        while (true)
        {
            staminaBar.value = player.staminaCurrentAmount / player.staminaMaxAmount;
            if (staminaBar.value > staminaUsageBar.value) staminaUsageBar.value = staminaBar.value;
            else staminaUsageBar.value = Mathf.Lerp(staminaUsageBar.value, staminaBar.value, Time.deltaTime * staminaUsageFollowSpeed);
            yield return null;
        }
    }
}
