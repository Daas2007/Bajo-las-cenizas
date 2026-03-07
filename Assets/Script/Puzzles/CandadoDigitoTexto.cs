// CandadoDigitoTexto.cs
using TMPro;
using UnityEngine;

public class CandadoDigitoTexto : MonoBehaviour
{
    [SerializeField] private TMP_Text texto; // texto que muestra el valor del dígito

    public void ActualizarTexto(int v)
    {
        if (texto) texto.text = v.ToString();
    }
}
