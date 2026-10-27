
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CineMachineEscenaaEscena : MonoBehaviour
{
    [SerializeField] PlayableDirector Cine;
    [SerializeField] GameObject Cine_Obj;
    [SerializeField] GameObject Gameplay_Obj;
    [SerializeField] GameObject MensajeParaSaltarCinematica;
    [SerializeField] string escenaSiguiente;
    [SerializeField] bool ConfirmacionParaSaltar = false;

    bool cambiarEscena = false;

    private void Awake()
    {
        if (MensajeParaSaltarCinematica != null)
        {
            MensajeParaSaltarCinematica.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (Cine != null)
        {
            Cine.stopped += OnCinematicaTerminada;
        }
    }

    private void OnDisable()
    {
        if (Cine != null)
        {
            Cine.stopped -= OnCinematicaTerminada;
        }
    }

    private void Start()
    {
        if (Cine != null)
        {
            cambiarEscena = false;
            Cine.time = 0;
            Cine.Play();
        }
    }

    private void Update()
    {
        CancelarCinematica();
    }

    private void OnCinematicaTerminada(PlayableDirector director)
    {
        if (cambiarEscena)
        {
            return;
        }

        cambiarEscena = true;
        SceneManager.LoadScene(escenaSiguiente);
    }

    private void CancelarCinematica()
    {
        if (Input.anyKeyDown && !ConfirmacionParaSaltar)
        {
            if (MensajeParaSaltarCinematica != null)
            {
                MensajeParaSaltarCinematica.SetActive(true);
            }

            ConfirmacionParaSaltar = true;
            return;
        }

        if (ConfirmacionParaSaltar &&
            Input.GetKeyDown(KeyCode.Space))
        {
            if (Cine != null)
            {
                Cine.Stop();
            }
        }
    }
}
