using UnityEngine;
using System.Collections;

public class JugadorLinterna : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] GameObject linternaObjeto;
    [SerializeField] GameObject luzLinterna;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip sonidoEncender;
    [SerializeField] Camera cam;

    [Header("Detección de enemigos")]
    [SerializeField] float distanciaMax = 10f;
    [SerializeField] LayerMask layerEnemigo;

    [Header("Bloqueo por diálogo")]
    [SerializeField] GameObject panelDialogo;

    [SerializeField] public bool tieneLinterna = false;
    [SerializeField] private EnemigoVentana enemigoDetectado;

    private Quaternion rotacionInicial;

    // 🔹 Referencia a MovimientoPersonaje
    MovimientoPersonaje movimientoJugador;

    void Awake()
    {
        movimientoJugador = FindObjectOfType<MovimientoPersonaje>();
    }

    void Start()
    {
        linternaObjeto.SetActive(false);
        luzLinterna.SetActive(false);
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        if (!tieneLinterna) return;
        if (panelDialogo != null && panelDialogo.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.F) && Time.deltaTime > 0)
        {
            ToggleLinterna();
        }

        if (luzLinterna.activeSelf)
        {
            DetectarEnemigoConLuz();
        }
        else if (enemigoDetectado != null)
        {
            enemigoDetectado.SetIluminado(false);
            enemigoDetectado = null;
        }
    }

    public void DarLinterna()
    {
        tieneLinterna = true;
        if (linternaObjeto != null) linternaObjeto.SetActive(true);
        if (luzLinterna != null) luzLinterna.SetActive(false);

        GameManager gm = GameManager.Instancia;
        if (gm != null) gm.tieneLinterna = true;

        TutorialInteractivo tutorial = FindObjectOfType<TutorialInteractivo>();
        if (tutorial != null) tutorial.NotificarLinternaRecogida();

        // 🔹 Activar flags en Animator
        if (movimientoJugador != null)
        {
            movimientoJugador.tieneLinterna = true;
            Animator anim = movimientoJugador.GetComponent<Animator>();
            anim.SetBool("TieneLinterna", true);
            anim.SetBool("AgarraLinterna", true);

            // 🔹 Reset inmediato para evitar bucle
            StartCoroutine(ResetBool(anim, "AgarraLinterna"));
        }

        Debug.Log("🔦 Linterna entregada al jugador.");
    }

    public void DarLinternaEncendida()
    {
        tieneLinterna = true;
        linternaObjeto.SetActive(true);
        luzLinterna.SetActive(true);

        if (source != null && sonidoEncender != null)
            source.PlayOneShot(sonidoEncender);

        TutorialInteractivo tutorial = FindObjectOfType<TutorialInteractivo>();
        if (tutorial != null) tutorial.NotificarLinternaRecogida();

        if (movimientoJugador != null)
        {
            movimientoJugador.tieneLinterna = true;
            Animator anim = movimientoJugador.GetComponent<Animator>();
            anim.SetBool("TieneLinterna", true);
            anim.SetBool("AgarraLinterna", true);

            StartCoroutine(ResetBool(anim, "AgarraLinterna"));
        }
    }

    private void ToggleLinterna()
    {
        bool nuevoEstado = !luzLinterna.activeSelf;
        luzLinterna.SetActive(nuevoEstado);

        if (source != null && sonidoEncender != null)
            source.PlayOneShot(sonidoEncender);

        TutorialInteractivo tutorial = FindObjectOfType<TutorialInteractivo>();
        if (tutorial != null && nuevoEstado) tutorial.SiguientePaso();
    }

    private void DetectarEnemigoConLuz()
    {
        Ray rayo = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaMax, layerEnemigo))
        {
            EnemigoVentana enemigo = hit.collider.GetComponent<EnemigoVentana>();
            if (enemigo != null)
            {
                enemigo.SetIluminado(true);
                enemigoDetectado = enemigo;
            }
        }
        else if (enemigoDetectado != null)
        {
            enemigoDetectado.SetIluminado(false);
            enemigoDetectado = null;
        }
    }

    public bool TieneLinterna()
    {
        return tieneLinterna;
    }

    public void ResetLinterna()
    {
        tieneLinterna = false;
        linternaObjeto.SetActive(false);
        luzLinterna.SetActive(false);

        if (movimientoJugador != null)
        {
            movimientoJugador.tieneLinterna = false;
            Animator anim = movimientoJugador.GetComponent<Animator>();
            anim.SetBool("TieneLinterna", false);
        }
    }

    // 🔹 Corrutina para resetear bool en Animator
    private IEnumerator ResetBool(Animator anim, string parametro)
    {
        yield return null; // esperar un frame
        anim.SetBool(parametro, false);
    }
}
