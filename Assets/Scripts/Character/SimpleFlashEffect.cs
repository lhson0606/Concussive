using System.Collections;
using UnityEngine;

// yoinked from https://github.com/BarthaSzabolcs/Tutorial-SpriteFlash/blob/main/Assets/Scripts/FlashEffects/SimpleFlash.cs
public class SimpleFlashEffect : MonoBehaviour
{
    [Tooltip("The duration of the flash effect")]
    [SerializeField] private float flashDuration = 0.1f;

    [Tooltip("The color of the flash effect")]
    [SerializeField] private Color flashColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}
