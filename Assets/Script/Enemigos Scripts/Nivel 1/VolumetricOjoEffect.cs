using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VolumetricOjoEffect : MonoBehaviour
{
    [Header("Material and Shader")]
    [Tooltip("Material that uses Hidden/VolumetricEye shader")]
    public Material effectMaterial;

    [Header("Center and shape")]
    [Tooltip("Normalized center on screen (0..1)")]
    public Vector2 center = new Vector2(0.5f, 0.5f);
    [Range(0.05f, 1f)]
    public float radius = 0.35f;
    [Range(0f, 1f)]
    public float softness = 0.45f;

    [Header("Visual")]
    public Color tint = new Color(1f, 0.05f, 0.05f, 1f);
    [Range(0f, 3f)]
    public float intensity = 1.0f;

    [Header("Noise")]
    public Texture2D noiseTexture;
    public float noiseScale = 3f;
    public float noiseSpeed = 0.6f;

    [Header("Pulse")]
    public float pulseSpeed = 6f;
    public float pulseAmplitude = 0.6f;
    public float baseIntensity = 0.2f;

    Coroutine pulseCoroutine;

    void Start()
    {
        if (effectMaterial == null)
        {
            Debug.LogWarning("[VolumetricOjoEffect] effectMaterial no asignado.");
            enabled = false;
            return;
        }

        // set defaults
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        effectMaterial.SetColor("_Color", tint);
        effectMaterial.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));
        effectMaterial.SetFloat("_Radius", radius);
        effectMaterial.SetFloat("_Softness", softness);
        effectMaterial.SetFloat("_Intensity", baseIntensity);
        if (noiseTexture != null) effectMaterial.SetTexture("_NoiseTex", noiseTexture);
        effectMaterial.SetFloat("_NoiseScale", noiseScale);
        effectMaterial.SetFloat("_NoiseSpeed", noiseSpeed);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        // pass time for noise animation
        effectMaterial.SetFloat("_TimeY", Time.time);
        Graphics.Blit(src, dest, effectMaterial);
    }

    // Public API to show a pulsing warning for duration seconds (if duration <= 0, pulse indefinitely)
    public void ShowPulse(float duration = -1f)
    {
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = StartCoroutine(PulseRoutine(duration));
    }

    public void HidePulse()
    {
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = null;
        // smoothly reduce intensity
        StartCoroutine(FadeIntensityTo(baseIntensity, 0.25f));
    }

    IEnumerator PulseRoutine(float duration)
    {
        float elapsed = 0f;
        while (duration <= 0f || elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // 0..1
            float target = baseIntensity + pulseAmplitude * t;
            effectMaterial.SetFloat("_Intensity", target);
            yield return null;
        }
        pulseCoroutine = null;
        StartCoroutine(FadeIntensityTo(baseIntensity, 0.25f));
    }

    IEnumerator FadeIntensityTo(float target, float time)
    {
        float start = effectMaterial.GetFloat("_Intensity");
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            effectMaterial.SetFloat("_Intensity", Mathf.Lerp(start, target, t / time));
            yield return null;
        }
        effectMaterial.SetFloat("_Intensity", target);
    }

    // Helper to quickly trigger a one-shot pulse
    public void OneShotPulse(float strength = 1f, float length = 0.6f)
    {
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = StartCoroutine(OneShotRoutine(strength, length));
    }

    IEnumerator OneShotRoutine(float strength, float length)
    {
        float half = length * 0.5f;
        // ramp up
        float t = 0f;
        float start = effectMaterial.GetFloat("_Intensity");
        while (t < half)
        {
            t += Time.deltaTime;
            effectMaterial.SetFloat("_Intensity", Mathf.Lerp(start, baseIntensity + strength, t / half));
            yield return null;
        }
        // ramp down
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            effectMaterial.SetFloat("_Intensity", Mathf.Lerp(baseIntensity + strength, baseIntensity, t / half));
            yield return null;
        }
        effectMaterial.SetFloat("_Intensity", baseIntensity);
        pulseCoroutine = null;
    }
}
