using UnityEngine;
using UnityEngine.UI;

public class LogicaBrillo : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] float sliderValor;
    [SerializeField] Image panelBrillo;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("brillo", 0.5f);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b,slider.value);

    }
    private void Update()
    {
    
    }

    public void CambiarSlider(float valor)
    {
        sliderValor = valor;
        PlayerPrefs.SetFloat("brillo", sliderValor);
        panelBrillo.color = new Color(panelBrillo.color.r,panelBrillo.color.g,panelBrillo.color.b, slider.value);
    }
}
