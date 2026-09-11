using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SimpleTextShadow : MonoBehaviour
{
    [Header("Shadow Settings")]
    private Color shadowColor = new Color32(132, 47, 40, 255);
    public Vector2 shadowOffset = new Vector2(6f, -6f);

    private Shadow shadowComponent;

    void Awake()
    {
        // Automatically find or add the built-in Unity Shadow component
        shadowComponent = GetComponent<Shadow>();
        if (shadowComponent == null)
        {
            shadowComponent = gameObject.AddComponent<Shadow>();
        }

        ApplyShadowSettings();
    }

    void OnValidate()
    {
        // This updates the shadow in real-time inside the Unity Editor when you adjust variables
        if (shadowComponent == null)
        {
            shadowComponent = GetComponent<Shadow>();
        }

        if (shadowComponent != null)
        {
            ApplyShadowSettings();
        }
    }

    private void ApplyShadowSettings()
    {
        shadowComponent.effectColor = shadowColor;
        shadowComponent.effectDistance = shadowOffset;
        shadowComponent.useGraphicAlpha = true;
    }
}
