
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Define los cuadros de dialogo disponibles.
public enum TipoCuadroDialogo
{
    Jugador,
    NPCGuia
}

// Guarda el texto y el cuadro que utilizara cada entrada.
[System.Serializable]
public class EntradaDialogo
{
    [TextArea(2, 5)]
    public string texto;

    public TipoCuadroDialogo tipoCuadro;
}

public class DialogoInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Dialogo principal")]
    [SerializeField] List<EntradaDialogo> dialogos = new List<EntradaDialogo>();

    [Header("Cuadros de dialogo")]
    [SerializeField] GameObject cuadroJugador;
    [SerializeField] GameObject cuadroNPCGuia;

    [Header("Textos")]
    [SerializeField] TMP_Text textoJugador;
    [SerializeField] TMP_Text textoNPCGuia;

    [Header("Sonido de escritura")]
    [SerializeField] float intervaloLetra = 0.04f;
    [SerializeField] AudioSource fuenteSonido;
    [SerializeField] AudioClip sonidoLetra;
    [SerializeField, Min(1)] int letrasMinimasPorSonido = 2;
    [SerializeField, Min(1)] int letrasMaximasPorSonido = 3;

    [Header("Dialogo repetido")]
    [SerializeField] bool usarDialogoRepetido = false;
    [SerializeField] EntradaDialogo dialogoRepetido;

    [SerializeField, Min(1)] int cantidadParaDialogoRepetido = 3;

    [Header("Contador")]
    [SerializeField] int contadorInteracciones = 0;

    // Indica si hay una conversacion abierta.
    bool dialogoActivo;

    // Indica si el texto esta apareciendo letra por letra.
    bool escribiendo;

    // Indica si el dialogo principal ya se completo.
    bool dialogoPrincipalLeido;

    // Indica si la entrada actual pertenece al dialogo repetido.
    bool mostrandoDialogoRepetido;

    // Indice de la entrada actual de la lista principal.
    int indiceDialogo;

    // Entrada que se esta mostrando actualmente.
    EntradaDialogo entradaActual;

    // Corrutina responsable de escribir las letras.
    Coroutine corrutinaEscritura;

    // Referencia al componente de texto activo.
    TMP_Text textoActual;

    // Indica si el jugador ya completo el dialogo principal al menos una vez.
    public bool HaHablado { get; private set; } = false;

    void Start()
    {
        // Oculta los cuadros al iniciar la escena.
        if (cuadroJugador != null)
            cuadroJugador.SetActive(false);

        if (cuadroNPCGuia != null)
            cuadroNPCGuia.SetActive(false);
    }

    void Update()
    {
        // El espacio controla el dialogo mientras esta abierto.
        // No hace falta comprobarlo si no hay una conversacion.
        if (!dialogoActivo)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ProcesarEspacio();
        }
    }

    // Metodo exigido por IInteractuable.
    public void Interactuar()
    {
        // Bloquea nuevas interacciones mientras el dialogo
        // esta abierto. El espacio se encarga de continuarlo.
        if (dialogoActivo)
            return;

        // Si ya se alcanzo el limite configurado,
        // comienza el dialogo repetido en futuras interacciones.
        if (usarDialogoRepetido &&
            contadorInteracciones >= cantidadParaDialogoRepetido)
        {
            IniciarDialogoRepetido();
            return;
        }

        // Si no se alcanzo el limite, comienza el principal.
        IniciarDialogoPrincipal();
    }

    // Inicia una conversacion principal desde su primera entrada.
    void IniciarDialogoPrincipal()
    {
        if (dialogos == null || dialogos.Count == 0)
            return;

        // Cada inicio real del dialogo principal suma uno.
        contadorInteracciones++;

        indiceDialogo = 0;
        dialogoActivo = true;
        dialogoPrincipalLeido = false;
        mostrandoDialogoRepetido = false;

        MostrarEntrada(dialogos[indiceDialogo]);
    }

    // Inicia el mensaje personalizado de las interacciones repetidas.
    void IniciarDialogoRepetido()
    {
        if (dialogoRepetido == null ||
            string.IsNullOrWhiteSpace(dialogoRepetido.texto))
        {
            return;
        }

        dialogoActivo = true;
        mostrandoDialogoRepetido = true;

        MostrarEntrada(dialogoRepetido);
    }

    // Activa el cuadro correspondiente y comienza a escribir.
    void MostrarEntrada(EntradaDialogo entrada)
    {
        entradaActual = entrada;

        bool esJugador =
            entrada.tipoCuadro == TipoCuadroDialogo.Jugador;

        if (cuadroJugador != null)
            cuadroJugador.SetActive(esJugador);

        if (cuadroNPCGuia != null)
            cuadroNPCGuia.SetActive(!esJugador);

        textoActual = esJugador ? textoJugador : textoNPCGuia;

        if (textoActual == null)
        {
            CerrarDialogo();
            return;
        }

        // Prepara el texto para revelar sus caracteres progresivamente.
        textoActual.text = entrada.texto;
        textoActual.maxVisibleCharacters = 0;

        corrutinaEscritura = StartCoroutine(EscribirTexto());
    }

    // Escribe el texto y reproduce sonidos de forma periodica.
    IEnumerator EscribirTexto()
    {
        escribiendo = true;

        int letrasDesdeSonido = 0;
        int siguienteSonido = ObtenerIntervaloSonido();

        for (int i = 0; i < textoActual.text.Length; i++)
        {
            // Revela el siguiente caracter.
            textoActual.maxVisibleCharacters = i + 1;

            char caracter = textoActual.text[i];

            // Los espacios no cuentan para el sonido.
            if (!char.IsWhiteSpace(caracter))
            {
                letrasDesdeSonido++;

                if (letrasDesdeSonido >= siguienteSonido)
                {
                    ReproducirSonidoLetra();

                    letrasDesdeSonido = 0;
                    siguienteSonido = ObtenerIntervaloSonido();
                }
            }

            yield return new WaitForSeconds(
                Mathf.Max(0f, intervaloLetra)
            );
        }

        // Garantiza que todo el texto quede visible.
        textoActual.maxVisibleCharacters = int.MaxValue;

        escribiendo = false;
        corrutinaEscritura = null;

        // Registra la lectura al terminar la ultima entrada principal.
        if (!mostrandoDialogoRepetido &&
            indiceDialogo >= dialogos.Count - 1)
        {
            dialogoPrincipalLeido = true;
            HaHablado = true;
        }
    }

    // Determina cuantas letras deben aparecer entre sonidos.
    int ObtenerIntervaloSonido()
    {
        int minimo = Mathf.Max(1, letrasMinimasPorSonido);
        int maximo = Mathf.Max(minimo, letrasMaximasPorSonido);

        return Random.Range(minimo, maximo + 1);
    }

    // Reproduce el sonido configurado para la escritura.
    void ReproducirSonidoLetra()
    {
        if (fuenteSonido != null && sonidoLetra != null)
        {
            fuenteSonido.PlayOneShot(sonidoLetra);
        }
    }

    // Procesa el uso de espacio para completar o avanzar el dialogo.
    void ProcesarEspacio()
    {
        if (escribiendo)
        {
            // Primer espacio: muestra el mensaje completo.
            CompletarEscritura();
        }
        else
        {
            // Siguiente espacio: avanza o cierra la conversacion.
            AvanzarDialogo();
        }
    }

    // Completa instantaneamente el mensaje actual.

    void CompletarEscritura()
    {
        if (!escribiendo)
            return;

        if (corrutinaEscritura != null)
        {
            StopCoroutine(corrutinaEscritura);
            corrutinaEscritura = null;
        }

        if (textoActual != null)
            textoActual.maxVisibleCharacters = int.MaxValue;

        escribiendo = false;

        // Si se completa la ultima entrada principal,
        // registra que el jugador ya hablo con el NPC.
        if (!mostrandoDialogoRepetido &&
            indiceDialogo >= dialogos.Count - 1)
        {
            dialogoPrincipalLeido = true;
            HaHablado = true;

            Debug.Log("[DialogoInteractuable] HaHablado = true");
        }
    }


    // Avanza a la siguiente entrada o cierra el dialogo.
    void AvanzarDialogo()
    {
        // El mensaje repetido termina al pulsar espacio otra vez.
        if (mostrandoDialogoRepetido)
        {
            CerrarDialogo();
            return;
        }

        indiceDialogo++;

        if (indiceDialogo < dialogos.Count)
        {
            MostrarEntrada(dialogos[indiceDialogo]);
        }
        else
        {
            CerrarDialogo();
        }
    }

    // Detiene la escritura y oculta los cuadros.
    void CerrarDialogo()
    {
        if (corrutinaEscritura != null)
        {
            StopCoroutine(corrutinaEscritura);
            corrutinaEscritura = null;
        }

        if (cuadroJugador != null)
            cuadroJugador.SetActive(false);

        if (cuadroNPCGuia != null)
            cuadroNPCGuia.SetActive(false);

        dialogoActivo = false;
        escribiendo = false;
        mostrandoDialogoRepetido = false;

        textoActual = null;
        entradaActual = null;
    }
}
