using UnityEngine;

public class ActivadorDeMinijuegos : MonoBehaviour, IInteractuable
{
    [Header("Referencias UI y Minijuego")]
    [SerializeField] private GameObject canvasDeMinijuego;

    [Tooltip("Arrastra aquí el GameObject que contiene el script del minijuego (debe implementar IMiniJuego)")]
    [SerializeField] private GameObject scriptMinijuegoObject;

    [Header("Referencia a la Cámara Principal")]
    [SerializeField] private NewCamara camaraScript;

    private IMiniJuego miniJuegoInterface;

    private void Awake()
    {
        if (canvasDeMinijuego != null)
        {
            canvasDeMinijuego.SetActive(false);
        }

        if (scriptMinijuegoObject != null)
        {
            miniJuegoInterface = scriptMinijuegoObject.GetComponent<IMiniJuego>();
        }
        else
        {
            miniJuegoInterface = GetComponent<IMiniJuego>();
        }

        if (camaraScript == null)
        {
            camaraScript = FindFirstObjectByType<NewCamara>();
        }
    }

    public void Interactuar()
    {
        if (canvasDeMinijuego != null)
        {
            canvasDeMinijuego.SetActive(true);
        }

        if (camaraScript != null)
        {
            camaraScript.EntrarModoUI();
        }
        else
        {
            Debug.LogWarning("⚠ No se encontró la referencia a NewCamara en ActivadorDeMinijuegos.");
        }

        if (miniJuegoInterface != null)
        {
            miniJuegoInterface.IniciarMiniJuego();
        }
        else
        {
            Debug.LogWarning($"⚠ No se encontró un componente que implemente IMiniJuego en {gameObject.name}.");
        }
    }

    public void CerrarMinijuego()
    {
        if (miniJuegoInterface != null)
        {
            miniJuegoInterface.FinalizarMiniJuego();
        }

        if (canvasDeMinijuego != null)
        {
            canvasDeMinijuego.SetActive(false);
        }

        if (camaraScript != null)
        {
            camaraScript.SalirModoUI();
        }
    }
}