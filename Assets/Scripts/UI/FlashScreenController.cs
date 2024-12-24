using System.Collections;
using UnityEngine;

public class FlashScreenController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup flashScreenCanvasGroup;
    [SerializeField]
    private float fadeDuration = 1f;
    [SerializeField]
    private float displayDuration = 2f;

    private GameObject currentScreen;

    private void Start()
    {
        flashScreenCanvasGroup.alpha = 0;
    }

    public void ShowFlashScreen(GameObject screenPrefab)
    {
        if (currentScreen != null)
        {
            Destroy(currentScreen);
        }

        currentScreen = Instantiate(screenPrefab, flashScreenCanvasGroup.transform);
        StartCoroutine(ShowFlashScreenCoroutine());
    }

    private IEnumerator ShowFlashScreenCoroutine()
    {
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            flashScreenCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        flashScreenCanvasGroup.alpha = 1;

        // Display for a duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            flashScreenCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        flashScreenCanvasGroup.alpha = 0;

        if (currentScreen != null)
        {
            Destroy(currentScreen);
        }
    }
}


