using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AlertaEnemigo : MonoBehaviour
{
    [SerializeField] GameObject AlertaDerechaEnemigo;
    [SerializeField] GameObject AlertaIzquierdaEnemigo;

    [SerializeField] AudioClip AudioAlertaClip;
    [SerializeField] AudioSource Audio;
    [SerializeField] float dadoAlerta;
    [SerializeField] float alertaImagen;


    private void Awake()
    {
        AlertaDerechaEnemigo.SetActive(false);
        AlertaIzquierdaEnemigo.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dadoAlerta = Random.Range(0, 20);
            alertaImagen = Random.Range(0, 10);
            if (dadoAlerta <= 10 && AudioAlertaClip != null)
            {
                TiempoAlerta();
                Destroy(this);
            }
        }
    }

    IEnumerator TiempoAlerta()
    {
        if (alertaImagen <= 5)
        {
            Audio.panStereo = -1.0f;
            Audio.PlayOneShot(AudioAlertaClip);
            AlertaIzquierdaEnemigo.SetActive(true);
            yield return new WaitForSeconds(3f);
            AlertaIzquierdaEnemigo.SetActive(false);
        }
        if (alertaImagen >= 6)
        {
            Audio.panStereo = 1.0f;
            Audio.PlayOneShot(AudioAlertaClip);
            AlertaDerechaEnemigo.SetActive(true);
            yield return new WaitForSeconds(3f);
            AlertaDerechaEnemigo.SetActive(false);
        }
        Audio.panStereo = 0f;
    }

}
