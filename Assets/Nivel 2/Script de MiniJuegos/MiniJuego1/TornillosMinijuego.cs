using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TornillosMinijuego : MonoBehaviour
{
    [SerializeField] int ClickMinimos = 70;
    [SerializeField] int ClickMaximos = 100;

    int clicksRestantes;
    Button BotonTornillo;
    MiniJuego1 controladorPrincipal;

    [SerializeField] AudioSource TornillosAlerta;
    [SerializeField] AudioClip ChirridoTornillo;
    [SerializeField] bool EnemigoEnCamino;

    void Awake()
    {
        BotonTornillo = GetComponent<Button>();
        GenerarClicsAleatorios();
        EnemigoEnCamino = false;
    }

    public void EstadoTornillo(MiniJuego1 controlador)
    {
        controladorPrincipal = controlador;

        GenerarClicsAleatorios();

        gameObject.SetActive(true);
        if (BotonTornillo != null) BotonTornillo.interactable = true;
    }

    public void ReiniciarTornillo(MiniJuego1 controlador)
    {
        controladorPrincipal = controlador;
        GenerarClicsAleatorios();

        gameObject.SetActive(true);
        if (BotonTornillo == null) BotonTornillo = GetComponent<Button>();
        BotonTornillo.interactable = true;
    }

    private void GenerarClicsAleatorios()
    {
        clicksRestantes = Random.Range(ClickMinimos, ClickMaximos + 1);
    }

    public void ProcesosDeClick()
    {
        if (clicksRestantes <= 0) return;

        clicksRestantes--;

        // FindObjectsInactive.Include asegura que lo encuentre aunque el padre haya estado desactivado
        if (controladorPrincipal == null)
        {
            controladorPrincipal = FindFirstObjectByType<MiniJuego1>(FindObjectsInactive.Include);
        }

        if (controladorPrincipal != null && controladorPrincipal.PuedeAlertar())
        {
            float numeroRnd = Random.Range(0f, 100f);

            if (numeroRnd >= 80f)
            {
                if (TornillosAlerta != null && ChirridoTornillo != null)
                {
                    TornillosAlerta.PlayOneShot(ChirridoTornillo);
                }

                EnemigoEnCamino = true;
                controladorPrincipal.ActivarAlertaEnemigo();
            }
        }

        if (clicksRestantes <= 0)
        {
            sumaTornillos();
            RetirarTornillos();
        }
    }


    void RetirarTornillos()
    {
        BotonTornillo.interactable = false;

        if (controladorPrincipal != null)
        {
            controladorPrincipal.NotificarTornilloRemovido();
        }

        gameObject.SetActive(false);
    }

    void sumaTornillos()
    {
        if (controladorPrincipal == null)
        {
            controladorPrincipal = FindFirstObjectByType<MiniJuego1>();
        }

        if (controladorPrincipal != null)
        {
            controladorPrincipal.ConteoTornillosSacados();
        }
        else
        {
            Debug.LogError("❌ No se encontró el script MiniJuego1 en la escena.");
        }
    }
}