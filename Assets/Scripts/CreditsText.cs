using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsText : MonoBehaviour
{
    public CanvasGroup[] uiGroups;
    public float fadeDuration = 1f;
    public float displayTime = 8f;

    private void Start()
    {
        // Initialize all Canvas Groups to be fully transparent and disabled
        foreach (var group in uiGroups)
        {
            SetAlpha(group, 0.0f);
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        StartCoroutine(FadeUIGroups());
    }

    private IEnumerator FadeUIGroups()
    {
        foreach (var group in uiGroups)
        {
            // Enable the group
            group.interactable = true;
            group.blocksRaycasts = true;

            // Fade in
            yield return StartCoroutine(FadeCanvasGroup(group, true));

            // Wait (including the fadeDuration to ensure 8 seconds of full visibility)
            yield return new WaitForSeconds(displayTime - (2 * fadeDuration));

            // Fade out
            yield return StartCoroutine(FadeCanvasGroup(group, false));

            // Disable the group
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, bool fadeIn)
    {
        group.gameObject.SetActive(true);
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        float alpha = group.alpha;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            SetAlpha(group, Mathf.Lerp(alpha, targetAlpha, t));
            yield return null;
        }

        SetAlpha(group, targetAlpha);
    }

    private void SetAlpha(CanvasGroup group, float alpha)
    {
        group.alpha = alpha;
    }
}
