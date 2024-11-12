using System.Collections;
using UnityEngine;

// yoinked from https://github.com/BarthaSzabolcs/Tutorial-SpriteFlash/blob/main/Assets/Scripts/FlashEffects/SimpleFlash.cs
public class SimpleFlashEffect : MonoBehaviour
{
    [Tooltip("The duration of the flash effect")]
    [SerializeField] private float flashDuration = 0.1f;

    [Tooltip("The material of the flash effect")]
    [SerializeField] private Material flashMaterial = null;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashCoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        if(flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
    }
}
