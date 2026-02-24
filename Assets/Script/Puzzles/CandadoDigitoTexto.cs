using TMPro;
using UnityEngine;

public class CandadoDigitoTexto : MonoBehaviour
{
    //---------------Referencia---------------
    [SerializeField] private TMP_Text texto; // texto que muestra el valor del dígito

    //---------------Actualizar texto---------------
    public void ActualizarTexto(int v)
    {
        if (texto) texto.text = v.ToString();
    }
}
