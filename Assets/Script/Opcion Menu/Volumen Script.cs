using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumenScript : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image imagenMute;
    [SerializeField] private AudioMixer audioMixer; // tu AudioMixer

    private float valorSlider;

    void Start()
    {
        // Cargar valor guardado o usar 0.5 por defecto
        valorSlider = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        slider.value = valorSlider;

        SetVolume(valorSlider);
        RevisarSiEstaEnMute();
    }

    public void CambiarSlider(float valor)
    {
        valorSlider = valor;
        PlayerPrefs.SetFloat("volumenAudio", valorSlider);

        SetVolume(valorSlider);
        RevisarSiEstaEnMute();
    }

    private void SetVolume(float valor)
    {
        if (valor <= 0.0001f)
        {
            audioMixer.SetFloat("MasterVolume", -80f); // silencio total
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(valor) * 20);
        }
    }

    private void RevisarSiEstaEnMute()
    {
        imagenMute.enabled = valorSlider <= 0.0001f;
    }
}
