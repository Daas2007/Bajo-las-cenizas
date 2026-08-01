using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cinematicaagameplay : MonoBehaviour
{
    [SerializeField] PlayableDirector Cine; //Objeto que contiene el "PlayableDirector"
    [SerializeField] float DuracionCinematica;
    float t;

    [SerializeField] GameObject Cine_Obj; //Objeto que contiene todo lo de la cinematica (todo lo que no se usara)
    [SerializeField] GameObject Gameplay_Obj; //Objeto que contiene todo lo del gameplay (todo lo que se usara) (usar un GameObject Empty para ordenar todo lo que si estara en el juego)

    void Start()
    {
        DuracionCinematica = (float)Cine.duration;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= DuracionCinematica)
        {
            Cine_Obj.SetActive(false);
            Gameplay_Obj.SetActive(true);
            Destroy(this);
        }
    }
}
