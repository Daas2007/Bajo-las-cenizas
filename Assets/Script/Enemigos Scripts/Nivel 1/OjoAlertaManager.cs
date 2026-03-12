using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OjoAlertaManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Imagen UI que representa el ojo entrecerrado o la máscara roja")]
    public Image ImagenOjo;

    [Header("Animación")]
    public float pulseSpeed = 6f;
    public float maxAlpha = 0.9f;
    public float minAlpha = 0.15f;
    public float scaleAmplitude = 0.08f;

    Coroutine pulseCoroutine;

    void Awake()
    {
        if (ImagenOjo != null)
        {
            SetAlpha(0f);
            ImagenOjo.gameObject.SetActive(false);
        }
    }

    // Muestra el efecto durante 'duracion' segundos. Si duracion <= 0, lo muestra indefinidamente hasta HideWarning.
    public void ShowWarning(float duracion = -1f)
    {
        if (ImagenOjo == null) return;

        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        ImagenOjo.gameObject.SetActive(true);
        pulseCoroutine = StartCoroutine(PulseRoutine(duracion));
    }

    public void HideWarning()
    {
        if (ImagenOjo == null) return;
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = null;
        StartCoroutine(FadeOutAndDisable());
    }

    IEnumerator PulseRoutine(float duracion)
    {
        float elapsed = 0f;
        while (duracion < 0f || elapsed < duracion)
        {
            elapsed += Time.deltaTime;

            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            SetAlpha(alpha);

            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * scaleAmplitude;
            ImagenOjo.rectTransform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        pulseCoroutine = null;
        StartCoroutine(FadeOutAndDisable());
    }

    IEnumerator FadeOutAndDisable()
    {
        float startAlpha = ImagenOjo.color.a;
        float duration = 0.25f;
        float t = 0f;
        Vector3 startScale = ImagenOjo.rectTransform.localScale;
        while (t < duration)
        {
            t += Time.deltaTime;
            float f = t / duration;
            SetAlpha(Mathf.Lerp(startAlpha, 0f, f));
            ImagenOjo.rectTransform.localScale = Vector3.Lerp(startScale, Vector3.one, f);
            yield return null;
        }

        SetAlpha(0f);
        ImagenOjo.rectTransform.localScale = Vector3.one;
        ImagenOjo.gameObject.SetActive(false);
    }

    void SetAlpha(float a)
    {
        if (ImagenOjo == null) return;
        Color c = ImagenOjo.color;
        c.a = Mathf.Clamp01(a);
        ImagenOjo.color = c;
    }
}
