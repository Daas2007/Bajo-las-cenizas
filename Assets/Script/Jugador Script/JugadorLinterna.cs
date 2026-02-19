using UnityEngine;

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
    [SerializeField] GameObject panelDialogo; // 👈 arrastra aquí tu panel de diálogo

    [SerializeField] public bool tieneLinterna = false;
    [SerializeField] private EnemigoVentana enemigoDetectado;

    void Start()
    {
        linternaObjeto.SetActive(false);
        luzLinterna.SetActive(false);
    }

    void Update()
    {
        if (!tieneLinterna) return;

        // 👇 Bloqueo: si el panel está activo, no permitir toggle
        if (panelDialogo != null && panelDialogo.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.F))
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
        linternaObjeto.SetActive(true);
        luzLinterna.SetActive(false);
    }

    public void DarLinternaEncendida()
    {
        tieneLinterna = true;
        linternaObjeto.SetActive(true);
        luzLinterna.SetActive(true);

        if (source != null && sonidoEncender != null)
            source.PlayOneShot(sonidoEncender);
    }

    private void ToggleLinterna()
    {
        bool nuevoEstado = !luzLinterna.activeSelf;
        luzLinterna.SetActive(nuevoEstado);

        if (source != null && sonidoEncender != null)
            source.PlayOneShot(sonidoEncender);
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
}
