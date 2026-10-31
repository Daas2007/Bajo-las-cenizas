using UnityEngine;

public class ActivadorDeMinijuegos : MonoBehaviour, IInteractuable
{
    [Header("Referencias UI y Minijuego")]
    [SerializeField] private GameObject canvasDeMinijuego;

    [Tooltip("Arrastra aquí el GameObject que contiene el script del minijuego (debe implementar IMiniJuego)")]
    [SerializeField] private GameObject scriptMinijuegoObject;

    [Header("Referencia a la Cámara Principal")]
    [SerializeField] private NewCamara camaraScript;

    [Header("Configuración del Televisor y Pistas (0 a 9)")]
    [Tooltip("Arrastra el Renderer del televisor donde cambiará el material.")]
    [SerializeField] private Renderer televisorRenderer;

    [Tooltip("Lista de 10 materiales que representan los números del 0 al 9.")]
    [SerializeField] private Material[] materialesNumeros = new Material[10];

    private IMiniJuego miniJuegoInterface;

    // Control para evitar que se repita el minijuego
    private bool minijuegoCompletado = false;

    // Almacena el número (0-9) que salió asignado en este televisor para el candado
    private int numeroAsignado = -1;

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
        // 2. Si ya se completó, evitamos que se vuelva a abrir
        if (minijuegoCompletado)
        {
            Debug.Log("Este minijuego ya fue completado y el televisor ya mostró su pista.");
            return;
        }

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

        if (!minijuegoCompletado)
        {
            minijuegoCompletado = true;
            AsignarMaterialAleatorioATelevisor();
        }

        // Guardar partida al regresar al nivel normal
        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.GuardarPartida();
            Debug.Log("Se Guardo Partida A Travez Del Minijuego");
        }
    }

    private void AsignarMaterialAleatorioATelevisor()
    {
        if (materialesNumeros != null && materialesNumeros.Length >= 10 && televisorRenderer != null)
        {
            numeroAsignado = Random.Range(0, 10);

            televisorRenderer.material = materialesNumeros[numeroAsignado];

            Debug.Log($"Minijuego completado. Número asignado para el candado: {numeroAsignado}");

            // TODO: Aquí puedes guardar este `numeroAsignado` en un GameManager o gestor de contraseñas 
            // para que el candado lo pueda leer al juntar los 3 minijuegos.
        }
    }

    public int GetNumeroAsignado()
    {
        return numeroAsignado;
    }
}