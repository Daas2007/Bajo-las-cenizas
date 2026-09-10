using UnityEngine;
using UnityEngine.Playables;

public class AnimationEnemyEntry : MonoBehaviour
{
    [Header("Referencias de Cinemática")]
    [SerializeField] private PlayableDirector Cine; // Objeto que contiene el PlayableDirector
    [SerializeField] private GameObject Cine_Obj;   // Contenedor de la cinemática
    [SerializeField] private GameObject Gameplay_Obj; // Contenedor del gameplay (Jugador / HUD)

    [Header("Referencia del Televisor")]
    [SerializeField] private TelevisorPrincipal tvPrincipal;

    private void OnEnable()
    {
        // Suscribirse al evento de finalización de Timeline
        if (Cine != null)
        {
            Cine.stopped += OnCinematicaTerminada;
        }

        // Suscribirse al evento de rotura del Televisor Principal
        if (tvPrincipal != null)
        {
            tvPrincipal.OnTelevisorRoto.AddListener(IniciarCinematicaEntrada);
        }
    }

    private void OnDisable()
    {
        if (Cine != null)
        {
            Cine.stopped -= OnCinematicaTerminada;
        }

        if (tvPrincipal != null)
        {
            tvPrincipal.OnTelevisorRoto.RemoveListener(IniciarCinematicaEntrada);
        }
    }

    /// <summary>
    /// Método invocado automáticamente cuando el Televisor Principal llega a 0 de vida.
    /// </summary>
    public void IniciarCinematicaEntrada()
    {
        Debug.Log("🎬 Iniciando cinemática de entrada del Enemigo Grande...");

        // Activa el objeto de la cinemática y desactiva el control de gameplay si es necesario
        if (Cine_Obj != null) Cine_Obj.SetActive(true);
        if (Gameplay_Obj != null) Gameplay_Obj.SetActive(false);

        // Reproduce el Timeline
        if (Cine != null)
        {
            Cine.Play();
        }
    }

    private void OnCinematicaTerminada(PlayableDirector director)
    {
        Debug.Log("🎬 Cinemática finalizada. Volviendo al gameplay...");

        if (Cine_Obj != null) Cine_Obj.SetActive(false);
        if (Gameplay_Obj != null) Gameplay_Obj.SetActive(true);

        // Desactivamos este script para evitar ejecuciones duplicadas
        enabled = false;
    }
}