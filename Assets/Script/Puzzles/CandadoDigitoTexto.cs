using TMPro;
using UnityEngine;

public class CandadoDigitoTexto : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;

    public void ActualizarTexto(int v)
    {
        if (texto) texto.text = v.ToString();
    }
}
