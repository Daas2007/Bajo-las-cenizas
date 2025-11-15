using UnityEngine;
using UnityEngine.UI;

public class VolumenScript : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] float valorSlider;
    [SerializeField] Image imagenMute;
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = slider.value;
        RevisarSiEstaEnMute();
    }
    public void CambiarSlider(float valor)
    {
        valorSlider = valor;
        PlayerPrefs.SetFloat("volumenAudio", valorSlider);
        AudioListener.volume = slider.value;
        RevisarSiEstaEnMute();
    }
    public void RevisarSiEstaEnMute()
    {
        if (valorSlider == 0)
        {
            imagenMute.enabled = true;
        }
        else
        {
            imagenMute.enabled = false;
        }
    }
}