using UnityEngine;
using UnityEngine.Events;

public class CandadoDigito : MonoBehaviour
{
    //---------------Configuración---------------
    [Range(0, 9)] public int valor = 0; // valor actual del dígito
    public int minimo = 0;              // valor mínimo permitido
    public int maximo = 9;              // valor máximo permitido

    //---------------Eventos---------------
    public UnityEvent<int> AlCambiarValor; // se dispara al cambiar el valor

    //---------------Incrementar---------------
    public void Incrementar()
    {
        valor = (valor + 1 > maximo) ? minimo : valor + 1;
        AlCambiarValor?.Invoke(valor);
    }

    //---------------Decrementar---------------
    public void Decrementar()
    {
        valor = (valor - 1 < minimo) ? maximo : valor - 1;
        AlCambiarValor?.Invoke(valor);
    }

    //---------------Establecer valor---------------
    public void EstablecerValor(int nuevoValor)
    {
        valor = Mathf.Clamp(nuevoValor, minimo, maximo);
        AlCambiarValor?.Invoke(valor);
    }
}
