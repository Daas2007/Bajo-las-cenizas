using UnityEngine;
using UnityEngine.Events;

public class CandadoDigito : MonoBehaviour
{
    [Range(0, 9)] public int valor = 0;
    public int minimo = 0;
    public int maximo = 9;

    public UnityEvent<int> AlCambiarValor;

    public void Incrementar()
    {
        valor = (valor + 1 > maximo) ? minimo : valor + 1;
        AlCambiarValor?.Invoke(valor);
    }

    public void Decrementar()
    {
        valor = (valor - 1 < minimo) ? maximo : valor - 1;
        AlCambiarValor?.Invoke(valor);
    }

    public void EstablecerValor(int nuevoValor)
    {
        valor = Mathf.Clamp(nuevoValor, minimo, maximo);
        AlCambiarValor?.Invoke(valor);
    }
}
