using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instancia;

    [Header("Referencias de UI")]
    [SerializeField] private GameObject marcoDialogo;
    [SerializeField] private TextMeshProUGUI textoNombre;
    [SerializeField] private TextMeshProUGUI textoMensaje;

    [Header("Ajustes")]
    [SerializeField] private float velocidadTexto = 0.04f;

    private Queue<string> frases = new Queue<string>();
    private Coroutine escribiendoCoroutine;
    private bool estaEscribiendo = false;
    private string fraseActual = "";
    private bool dialogoActivo = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Si el diálogo está activo y se presiona ESPACIO, avanza a la siguiente frase
        if (dialogoActivo && Input.GetKeyDown(KeyCode.Space))
        {
            SiguienteFrase();
        }
    }

    public void IniciarDialogo(string nombre, string[] lineas)
    {
        dialogoActivo = true;
        marcoDialogo.SetActive(true);
        textoNombre.text = nombre;

        // Pausar o bloquear movimiento/cámara si es necesario
        BloquearControlesJugador(true);

        frases.Clear();
        foreach (string linea in lineas)
        {
            frases.Enqueue(linea);
        }

        SiguienteFrase();
    }

    public void SiguienteFrase()
    {
        // Si el texto se está escribiendo y se pulsa Espacio, muestra la frase completa de golpe
        if (estaEscribiendo)
        {
            if (escribiendoCoroutine != null) StopCoroutine(escribiendoCoroutine);
            textoMensaje.text = fraseActual;
            estaEscribiendo = false;
            return;
        }

        // Si ya no quedan más frases, cierra el diálogo
        if (frases.Count == 0)
        {
            CerrarDialogo();
            return;
        }

        fraseActual = frases.Dequeue();
        escribiendoCoroutine = StartCoroutine(EscribirFrase(fraseActual));
    }

    IEnumerator EscribirFrase(string frase)
    {
        estaEscribiendo = true;
        textoMensaje.text = "";

        foreach (char letra in frase.ToCharArray())
        {
            textoMensaje.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        estaEscribiendo = false;
    }

    public void CerrarDialogo()
    {
        dialogoActivo = false;
        marcoDialogo.SetActive(false);
        BloquearControlesJugador(false);
    }

    private void BloquearControlesJugador(bool bloquear)
    {
        MovimientoPersonaje movimiento = FindObjectOfType<MovimientoPersonaje>();
        if (movimiento != null) movimiento.enabled = !bloquear;

        Camara camara = FindObjectOfType<Camara>();
        if (camara != null) camara.enabled = !bloquear;
    }
}