using UnityEngine;
using UnityEngine.Playables;

public class Cinematicaagameplay : MonoBehaviour
{
    [SerializeField] PlayableDirector Cine; // Objeto que contiene el PlayableDirector
    [SerializeField] GameObject Cine_Obj;   // Contenedor de la cinemática
    [SerializeField] GameObject Gameplay_Obj; // Contenedor del gameplay

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

    void OnCinematicaTerminada(PlayableDirector director)
    {
        Cine_Obj.SetActive(false);
        Gameplay_Obj.SetActive(true);

        // Desactivamos el GameObject o el script de forma segura
        enabled = false;
    }
}