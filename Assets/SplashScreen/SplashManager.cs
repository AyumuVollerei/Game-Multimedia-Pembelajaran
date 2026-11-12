using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Wajib untuk mengatur waktu bertahap

public class SplashManager : MonoBehaviour
{
    public float delayTime = 4f; // Waktu tunggu sebelum mulai memudar
    public float fadeDuration = 2f; // Lama waktu memudar (1 detik)
    public CanvasGroup canvasGroup; // Tempat Canvas kita nanti

    void Start()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        yield return new WaitForSeconds(delayTime);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}