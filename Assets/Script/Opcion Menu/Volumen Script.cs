using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumenScript : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfectos;
    [SerializeField] private Image imagenMute;
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        // Cargar valores guardados o usar 0.5 por defecto
        float volumenGeneral = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        float volumenMusica = PlayerPrefs.GetFloat("volumenMusica", 0.5f);
        float volumenEfectos = PlayerPrefs.GetFloat("volumenEfectos", 0.5f);

        // Asignar valores a los sliders
        sliderMaster.value = volumenGeneral;
        sliderMusica.value = volumenMusica;
        sliderEfectos.value = volumenEfectos;

        // Aplicar valores al AudioMixer
        SetVolume(volumenGeneral);
        SetMusica(volumenMusica);
        SetEfectos(volumenEfectos);

        RevisarSiEstaEnMute(volumenGeneral);
    }

    public void CambiarSliderGeneral(float valor)
    {
        PlayerPrefs.SetFloat("volumenAudio", valor);
        SetVolume(valor);
        RevisarSiEstaEnMute(valor);
    }

    public void CambiarSliderMusica(float valor)
    {
        PlayerPrefs.SetFloat("volumenMusica", valor);
        SetMusica(valor);
    }

    public void CambiarSliderEfectos(float valor)
    {
        PlayerPrefs.SetFloat("volumenEfectos", valor);
        SetEfectos(valor);
    }

    private void SetVolume(float valor)
    {
        if (valor <= 0.0001f)
            audioMixer.SetFloat("MasterVolume", -80f);
        else
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(valor) * 20);
    }

    private void SetMusica(float valor)
    {
        if (valor <= 0.0001f)
            audioMixer.SetFloat("MusicVolume", -80f);
        else
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(valor) * 20);
    }

    private void SetEfectos(float valor)
    {
        if (valor <= 0.0001f)
            audioMixer.SetFloat("SFXVolume", -80f);
        else
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(valor) * 20);
    }

    private void RevisarSiEstaEnMute(float valor)
    {
        imagenMute.enabled = valor <= 0.0001f;
    }
}
