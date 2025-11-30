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
    private bool tieneLinterna = false; // 🔑 nuevo control

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        luzLinterna.SetActive(false);

        // Revisar si ya estaba guardado que el jugador tiene linterna
        if (PlayerPrefs.GetInt("TieneLinterna", 0) == 1)
        {
            tieneLinterna = true;
        }
    }

    private void Update()
    {
        if (tieneLinterna) // 🔑 solo si la tiene
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
    }

    public void DarLinterna() // 🔑 llamado desde el pickup
    {
        tieneLinterna = true;
        PlayerPrefs.SetInt("TieneLinterna", 1);
        PlayerPrefs.Save();
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
