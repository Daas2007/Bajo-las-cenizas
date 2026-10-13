using UnityEngine;
using UnityEngine.Events;
using TMPro; // Asegúrate de tener instalado TextMeshPro

public class CandadoCombinacion : MonoBehaviour, IInteractuable
{
    [Header("Referencias UI y Cámara")]
    [Tooltip("El Canvas o Panel de la UI que se abrirá al interactuar con el candado.")]
    [SerializeField] private GameObject canvasDeCandado;

    [Tooltip("El InputField (TextMeshPro) donde el jugador escribe el código.")]
    [SerializeField] private TMP_InputField inputFieldCodigo;

    [Tooltip("Referencia a la cámara principal para el modo UI.")]
    [SerializeField] private NewCamara camaraScript;

    [Header("Referencias de Minijuegos")]
    [Tooltip("Deja esto vacío si los televisores están en la escena y quieres que los busque automáticamente.")]
    [SerializeField] private ActivadorDeMinijuegos[] televisores;

    [Header("Eventos")]
    [Tooltip("Acciones que ocurren cuando la contraseña es correcta (ej. abrir una puerta, destruir el candado, etc.)")]
    [SerializeField] private UnityEvent onCandadoCorrecto;

    [Tooltip("Acciones que ocurren cuando la contraseña es incorrecta.")]
    [SerializeField] private UnityEvent onCandadoIncorrecto;

    private string codigoSecreto = "";
    private bool candadoAbierto = false;

    private void Awake()
    {
        // Asegurarnos de que el canvas empiece desactivado
        if (canvasDeCandado != null)
        {
            canvasDeCandado.SetActive(false);
        }

        // Buscar la cámara automáticamente si no está asignada
        if (camaraScript == null)
        {
            camaraScript = FindFirstObjectByType<NewCamara>();
        }
    }

    private void Start()
    {
        // Si no asignaste los televisores manualmente en el Inspector, los buscamos todos en la escena
        if (televisores == null || televisores.Length == 0)
        {
            televisores = FindObjectsByType<ActivadorDeMinijuegos>(FindObjectsSortMode.None);
        }

        GenerarCodigoSecreto();
    }

    /// <summary>
    /// Recorre los televisores para extraer sus números asignados y armar la contraseña de 3 dígitos.
    /// </summary>
    public void GenerarCodigoSecreto()
    {
        codigoSecreto = "";

        foreach (var tv in televisores)
        {
            if (tv != null)
            {
                int numero = tv.GetNumeroAsignado();

                if (numero != -1)
                {
                    codigoSecreto += numero.ToString();
                }
                else
                {
                    codigoSecreto += "?";
                }
            }
        }

        Debug.Log($"🔐 Código secreto actual del candado: {codigoSecreto}");
    }

    /// <summary>
    /// Método de la interfaz IInteractuable. Abre el Canvas y pone la cámara en modo UI.
    /// </summary>
    public void Interactuar()
    {
        if (candadoAbierto)
        {
            Debug.Log("El candado ya está abierto.");
            return;
        }

        // Actualizamos el código por si acaso el jugador completó algún minijuego recientemente
        GenerarCodigoSecreto();

        // Activamos la interfaz del candado
        if (canvasDeCandado != null)
        {
            canvasDeCandado.SetActive(true);
        }

        // Cambiamos el estado de la cámara para liberar el mouse / pausar movimiento
        if (camaraScript != null)
        {
            camaraScript.EntrarModoUI();
        }
        else
        {
            Debug.LogWarning("⚠ No se encontró la referencia a NewCamara en CandadoCombinacion.");
        }

        // Limpiamos el input field al abrir
        if (inputFieldCodigo != null)
        {
            inputFieldCodigo.text = "";
        }
    }

    /// <summary>
    /// Método para cerrar el candado manualmente (por ejemplo, con un botón "X" o "Salir" en la UI).
    /// </summary>
    public void CerrarCandado()
    {
        if (canvasDeCandado != null)
        {
            canvasDeCandado.SetActive(false);
        }

        if (camaraScript != null)
        {
            camaraScript.SalirModoUI();
        }
    }

    /// <summary>
    /// Método que se conecta al botón de "Comprobar" o al evento OnEndEdit del InputField.
    /// Lee lo que escribió el jugador y valida si es correcto.
    /// </summary>
    public void ComprobarCodigoIngresado()
    {
        if (candadoAbierto || inputFieldCodigo == null) return;

        string codigoIngresado = inputFieldCodigo.text;

        // Volvemos a generar el código por seguridad
        GenerarCodigoSecreto();

        if (codigoIngresado == codigoSecreto)
        {
            candadoAbierto = true;
            Debug.Log("¡Contraseña correcta! El candado se ha abierto.");

            // Cerramos la UI y regresamos la cámara a la normalidad
            CerrarCandado();

            // Ejecuta los eventos configurados en el Inspector (abrir puerta, sonido, etc.)
            onCandadoCorrecto?.Invoke();
        }
        else
        {
            Debug.Log($"Contraseña incorrecta. Ingresaste: {codigoIngresado}, pero el código correcto es: {codigoSecreto}");

            // Ejecuta los eventos de error (sonido de error, vibración, etc.)
            onCandadoIncorrecto?.Invoke();
        }
    }
}