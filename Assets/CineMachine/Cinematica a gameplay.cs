using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Cinematicaagameplay : MonoBehaviour
{
    [SerializeField] PlayableDirector Cine; // Objeto que contiene el PlayableDirector
    [SerializeField] GameObject Cine_Obj;   // Contenedor de la cinemática
    [SerializeField] GameObject Gameplay_Obj; // Contenedor del gameplay
    [SerializeField] GameObject MensajeParaSaltarCinematica;
    [SerializeField] bool ConfirmacionParaSaltar = false;

    private void Awake()
    {
        MensajeParaSaltarCinematica.SetActive(false);
    }
    void OnEnable()
    {
        if (Cine != null)
        {
            Cine.stopped += OnCinematicaTerminada;
        }
    }

    void OnDisable()
    {
        if (Cine != null)
        {
            Cine.stopped -= OnCinematicaTerminada;
        }
    }

    private void Update()
    {
        CancelarCinematica();
    }
    void OnCinematicaTerminada(PlayableDirector director)
    {
        Cine_Obj.SetActive(false);
        Gameplay_Obj.SetActive(true);
        MensajeParaSaltarCinematica.SetActive(false);

        // Desactivamos el GameObject o el script de forma segura
        enabled = false;
    }
    void CancelarCinematica()
    {
        if (Input.anyKeyDown && !ConfirmacionParaSaltar)
        {
            MensajeParaSaltarCinematica.SetActive(true);
            ConfirmacionParaSaltar = true;
            return;
        }
        if (ConfirmacionParaSaltar && Input.GetKeyDown(KeyCode.Space))
        {
            OnCinematicaTerminada(Cine);
        }
    }
}