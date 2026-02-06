using UnityEngine;

public class JugadorLinterna : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] GameObject linternaObjeto;
    [SerializeField] GameObject luzLinterna;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip sonidoEncender;

    private bool tieneLinterna = false;

    void Start()
    {
        linternaObjeto.SetActive(false);
        luzLinterna.SetActive(false);

        if (PlayerPrefs.GetInt("TieneLinterna", 0) == 1)
        {
            DarLinterna(); // si estaba guardado
        }
    }

    void Update()
    {
        if (!tieneLinterna) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleLinterna();
        }
    }

    public void DarLinterna()
    {
        tieneLinterna = true;
        linternaObjeto.SetActive(true);
        luzLinterna.SetActive(false);

        PlayerPrefs.SetInt("TieneLinterna", 1);
        PlayerPrefs.Save();
    }

    // Nuevo: recoger linterna y encenderla directamente
    public void DarLinternaEncendida()
    {
        tieneLinterna = true;
        linternaObjeto.SetActive(true);
        luzLinterna.SetActive(true);

        PlayerPrefs.SetInt("TieneLinterna", 1);
        PlayerPrefs.Save();

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
}
