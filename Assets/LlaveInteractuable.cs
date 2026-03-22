using UnityEngine;

public class LlaveInteractuable : MonoBehaviour, IInteractuable
{
    [SerializeField] private CandadoPuerta candado;
    private bool enMano = false;
    private Transform manoIzquierda;

    void Start()
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
            if (interaccion != null)
                manoIzquierda = interaccion.GetManoIzquierda();
        }
    }

    public void Interactuar()
    {
        if (!enMano && manoIzquierda != null && manoIzquierda.childCount == 0)
        {
            enMano = true;
            transform.SetParent(manoIzquierda);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public bool EstaEnMano()
    {
        return enMano;
    }

    public void UsarEnCandado()
    {
        if (candado != null)
        {
            candado.DestruirCandado();
            Destroy(gameObject); // destruir llave
        }
    }
}
