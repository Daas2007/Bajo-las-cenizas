using UnityEngine;

public class Linterna : MonoBehaviour
{
    [Header("Componentes de la Linterna")]
    [SerializeField] GameObject luzLinterna;
    [SerializeField] AudioClip LuzSonido;
    [SerializeField] private AudioSource source;

    [Header("Sistema Deteccion Enemigo")]
    [SerializeField] float distanciaMax = 10f;
    [SerializeField] LayerMask layerEnemigo;

    [SerializeField] Camera cam;

    private EnemigoVentana enemigoDetectato;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        luzLinterna.SetActive(false);
    }
    private void Update()
    {
        InterracionLinterna();
        if (luzLinterna.activeSelf)
        {
            DetectarEnemigoConLuz();
        }
        else
        {
            if (enemigoDetectato != null)
                enemigoDetectato.SetIluminado(false);
        }
    }
    public void InterracionLinterna()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LuzManager();
        }
    }
    public void LuzManager()
    {
        luzLinterna.SetActive(!luzLinterna.activeSelf);
        source.PlayOneShot(LuzSonido);
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
                enemigoDetectato = enemigo;
                // Debug.DrawRay(cam.transform.position, cam.transform.forward * distanciaMax, Color.yellow);
            }
        }
        else
        {
            if (enemigoDetectato != null)
            {
                enemigoDetectato.SetIluminado(false);
                enemigoDetectato = null;
            }
        }
    }
}
