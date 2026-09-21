using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniJuego1 : MonoBehaviour, IMiniJuego
{
    [Header("UI")]
    [SerializeField] GameObject canvasMinijuego;
    [SerializeField] GameObject botonTerminarMinijuego;

    [Header("Coleccion De Tornillos")]
    [SerializeField] TornillosMinijuego[] listaTornillos;
    [SerializeField] TornillosMinijuego tornillos;

    [Header("Conteo de tornillos")]
    [SerializeField] int TornillosSacados = 0;
    int tornillosRestantes;

    [Header("Utilidades Cambio Escenario")]
    [SerializeField] GameObject FondoNormal;   
    [SerializeField] GameObject FondoSinEnemigo;   
    [SerializeField] GameObject FondoConEnemigo; 

    [Header("Screamer Assets")]
    [SerializeField] GameObject fondoScreamer;      
    [SerializeField] AudioSource audioSourceScreamer; 
    [SerializeField] AudioClip sonidoScreamer; 

    [Header("Estado del jugador")]
    [SerializeField] bool estaEnCama = false;
    [SerializeField] bool alertaActiva = false;

    private Coroutine rutinaAtaque;

    private void Awake()
    {
        if (botonTerminarMinijuego != null)
        {
            botonTerminarMinijuego.SetActive(false);
        }

        ResetearFondos();
    }

    private void OnEnable()
    {
        IniciarMiniJuego();
    }

    public void IniciarMiniJuego()
    {
        if (canvasMinijuego != null)
        {
            canvasMinijuego.SetActive(true);
        }

        TornillosSacados = 0;
        estaEnCama = false;
        alertaActiva = false;

        ResetearFondos();
        if (FondoNormal != null)
        {
            FondoNormal.SetActive(true);
        }

        Camara cam = Camera.main != null ? Camera.main.GetComponent<Camara>() : FindFirstObjectByType<Camara>();
        if (cam != null)
        {
            cam.SetModoUI(true);
        }

        if (botonTerminarMinijuego != null)
        {
            botonTerminarMinijuego.SetActive(false);
        }

        tornillosRestantes = listaTornillos.Length;
        foreach (TornillosMinijuego tornillo in listaTornillos)
        {
            if (tornillo != null)
            {
                tornillo.EstadoTornillo(this);
            }
        }
    }

    public void CambioDeFondo()
    {
        estaEnCama = !estaEnCama;

        if (estaEnCama)
        {
            if (FondoNormal != null) FondoNormal.SetActive(false);
            if (FondoConEnemigo != null) FondoConEnemigo.SetActive(false);
            if (FondoSinEnemigo != null) FondoSinEnemigo.SetActive(true);
        }
        else
        {
            if (FondoSinEnemigo != null) FondoSinEnemigo.SetActive(false);
            if (FondoConEnemigo != null) FondoConEnemigo.SetActive(false);
            if (FondoNormal != null) FondoNormal.SetActive(true);
        }
    }

    public void ActivarAlertaEnemigo()
    {
        if (alertaActiva) return;

        alertaActiva = true;
        if (rutinaAtaque != null) StopCoroutine(rutinaAtaque);
        rutinaAtaque = StartCoroutine(AtaqueEnemigo());
    }

    private IEnumerator AtaqueEnemigo()
    {
        yield return new WaitForSeconds(4f);

        if (estaEnCama)
        {
            if (FondoSinEnemigo != null) FondoSinEnemigo.SetActive(false);
            if (FondoConEnemigo != null) FondoConEnemigo.SetActive(true);

            yield return new WaitForSeconds(4f);

            if (FondoConEnemigo != null) FondoConEnemigo.SetActive(false);
            if (FondoSinEnemigo != null) FondoSinEnemigo.SetActive(true);

            yield return new WaitForSeconds(4f);
            alertaActiva = false;
        }
        else
        {
            StartCoroutine(EjecutarScreamer());
        }
    }

    private IEnumerator EjecutarScreamer()
    {
        ResetearFondos();

        if (fondoScreamer != null)
        {
            fondoScreamer.SetActive(true);
        }

        if (audioSourceScreamer != null && sonidoScreamer != null)
        {
            audioSourceScreamer.PlayOneShot(sonidoScreamer);
        }

        yield return new WaitForSeconds(2.3f);

        if (fondoScreamer != null)
        {
            fondoScreamer.SetActive(false);
        }

        ReiniciarMinijuegoCompleto();
    }

    public void ReiniciarMinijuegoCompleto()
    {
        if (rutinaAtaque != null) StopCoroutine(rutinaAtaque);

        TornillosSacados = 0;
        tornillosRestantes = listaTornillos.Length;
        estaEnCama = false;
        alertaActiva = false;

        ResetearFondos();

        if (FondoNormal != null)
        {
            FondoNormal.SetActive(true);
        }

        if (botonTerminarMinijuego != null)
        {
            botonTerminarMinijuego.SetActive(false);
        }

        foreach (TornillosMinijuego tornillo in listaTornillos)
        {
            if (tornillo != null)
            {
                tornillo.ReiniciarTornillo(this);
            }
        }

        Debug.Log("🔄 Minijuego reiniciado por Screamer.");
    }

    private void ResetearFondos()
    {
        if (FondoNormal != null) FondoNormal.SetActive(false);
        if (FondoSinEnemigo != null) FondoSinEnemigo.SetActive(false);
        if (FondoConEnemigo != null) FondoConEnemigo.SetActive(false);
        if (fondoScreamer != null) fondoScreamer.SetActive(false);
    }

    public void NotificarTornilloRemovido()
    {
        tornillosRestantes--;

        if (TornillosSacados >= listaTornillos.Length)
        {
            ActivarBotonFinal();
        }
    }

    public void ActivarBotonFinal()
    {
        if (botonTerminarMinijuego != null)
        {
            botonTerminarMinijuego.SetActive(true);
            Debug.Log("✅ Botón de finalizar activado con éxito.");
        }
    }

    public void FinalizarMiniJuego()
    {
        Camara cam = Camera.main != null ? Camera.main.GetComponent<Camara>() : FindFirstObjectByType<Camara>();
        if (cam != null)
        {
            cam.SetModoUI(false);
        }

        if (rutinaAtaque != null) StopCoroutine(rutinaAtaque);
        canvasMinijuego.SetActive(false);
    }

    public void ConteoTornillosSacados()
    {
        TornillosSacados++;
        if (TornillosSacados >= listaTornillos.Length)
        {
            ActivarBotonFinal();
        }
        Debug.Log($"🔩 Tornillos Sacados: {TornillosSacados} / {listaTornillos.Length}");
    }

    public bool PuedeAlertar()
    {
        return !alertaActiva;
    }
}