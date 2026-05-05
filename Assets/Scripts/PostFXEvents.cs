using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostFXEvents : MonoBehaviour
{
    public static PostFXEvents Instance { get; private set; }

    [SerializeField] private Volume volume;

    [Header("Damage Flash")]
    public Color damageColor = new Color(0.9f, 0.05f, 0.05f);
    public float damagePeakIntensity = 0.75f;
    public float damageDuration = 0.35f;

    [Header("Low HP Pulse")]
    public Color lowHpColor = new Color(0.55f, 0f, 0f);
    public float lowHpPeakIntensity = 0.6f;
    public float lowHpPulseSpeed = 3f;

    private Vignette vignette;
    private Color baseColor;
    private float baseIntensity;
    private float baseSmoothness;

    private Coroutine activeFlash;
    private Coroutine activeLowHp;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        if (volume == null) volume = GetComponent<Volume>();
        if (volume == null || volume.sharedProfile == null) return;

        if (volume.sharedProfile.TryGet(out Vignette v))
        {
            vignette = v;
            baseColor = vignette.color.value;
            baseIntensity = vignette.intensity.value;
            baseSmoothness = vignette.smoothness.value;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        ResetVignette();
    }

    public void DamageFlash()
    {
        if (vignette == null) return;
        if (activeFlash != null) StopCoroutine(activeFlash);
        activeFlash = StartCoroutine(FlashRoutine(damageColor, damagePeakIntensity, damageDuration));
    }

    public void SetLowHP(bool active)
    {
        if (vignette == null) return;
        if (activeLowHp != null) { StopCoroutine(activeLowHp); activeLowHp = null; }
        if (active) activeLowHp = StartCoroutine(LowHPPulse());
        else ResetVignette();
    }

    IEnumerator FlashRoutine(Color flashColor, float peakIntensity, float duration)
    {
        float half = Mathf.Max(0.01f, duration * 0.5f);
        Color startColor = vignette.color.value;
        float startIntensity = vignette.intensity.value;

        float t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / half);
            vignette.color.value = Color.Lerp(startColor, flashColor, p);
            vignette.intensity.value = Mathf.Lerp(startIntensity, peakIntensity, p);
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / half);
            vignette.color.value = Color.Lerp(flashColor, baseColor, p);
            vignette.intensity.value = Mathf.Lerp(peakIntensity, baseIntensity, p);
            yield return null;
        }
        ResetVignette();
        activeFlash = null;
    }

    IEnumerator LowHPPulse()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * lowHpPulseSpeed) + 1f) * 0.5f;
            vignette.color.value = Color.Lerp(baseColor, lowHpColor, t);
            vignette.intensity.value = Mathf.Lerp(baseIntensity, lowHpPeakIntensity, t);
            yield return null;
        }
    }

    void ResetVignette()
    {
        if (vignette == null) return;
        vignette.color.value = baseColor;
        vignette.intensity.value = baseIntensity;
        vignette.smoothness.value = baseSmoothness;
    }
}
