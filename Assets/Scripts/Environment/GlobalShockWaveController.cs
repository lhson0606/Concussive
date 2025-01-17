using System.Collections;
using UnityEngine;

public class GlobalShockWaveController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    public void DoShockWave(Vector2 position, float speed, float duration)
    {
        StartCoroutine(StartShockWave(position, speed, duration));
    }

    private IEnumerator StartShockWave(Vector3 position, float speed, float duration)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(position);

        float time = 0;
        float distanceFromCenter = 0;
        material.SetVector("_RingSpawnPosition", viewportPosition);
        while (time < duration)
        {
            distanceFromCenter = Mathf.Lerp(0, 1, time / duration);
            material.SetFloat("_WaveDistanceFromCenter", distanceFromCenter);
            time += Time.deltaTime;
            yield return null;
        }

        material.SetFloat("_WaveDistanceFromCenter", -0.1f);
    }
}
