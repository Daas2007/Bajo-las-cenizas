using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LogicaPantallaCompleta : MonoBehaviour
{
    [SerializeField] Toggle togglePantallaCompleta;
    [SerializeField] TMP_Dropdown resolucionesDropDown;

    Resolution[] resoluciones;

    void Start()
    {
        togglePantallaCompleta.isOn = Screen.fullScreen;
        RevisarResoluciones();
    }

    public void ActivarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    public void RevisarResoluciones()
    {
        // 🔧 Filtrar resoluciones únicas por ancho y alto
        resoluciones = Screen.resolutions
            .GroupBy(r => new { r.width, r.height }) // agrupar por tamaño
            .Select(g => g.First())                  // tomar solo una versión
            .ToArray();

        resolucionesDropDown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }

        resolucionesDropDown.AddOptions(opciones);
        resolucionesDropDown.value = resolucionActual;
        resolucionesDropDown.RefreshShownValue();
    }

    public void CambiarResolucion(int indiceResolucion)
    {
        Resolution resolucion = resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }
}
