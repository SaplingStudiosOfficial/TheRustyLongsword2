using UnityEngine;
using System.Collections;

public class ShakeOnEnable : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.35f;
    [SerializeField] private float shakeAmount = 12f;
    [SerializeField] private float shakeSpeed = 35f;

    private Quaternion originalRotation;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        originalRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(Shake());
    }

    private void OnDisable()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localRotation = originalRotation;
    }

    private IEnumerator Shake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            // Shake gets weaker as it approaches the end.
            float strength =
                1f - (timer / shakeDuration);

            float rotation =
                Mathf.Sin(timer * shakeSpeed)
                * shakeAmount
                * strength;

            transform.localRotation =
                originalRotation
                * Quaternion.Euler(
                    0f,
                    0f,
                    rotation
                );

            yield return null;
        }

        transform.localRotation = originalRotation;

        shakeCoroutine = null;
    }
}