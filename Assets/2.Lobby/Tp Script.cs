using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class TpScript : MonoBehaviour
{
    [SerializeField] GameObject cinematicaPortal;

    [SerializeField] GameObject playableObject;

    [SerializeField] GameObject Jugador;

    [SerializeField] GameObject referenciaTP;

    private void Awake()
    {
        cinematicaPortal.SetActive(false);
        playableObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cinematicaPortal.SetActive(true);
            if (other.CompareTag("Player") && cinematicaPortal != null)
            {
                playableObject.SetActive(true);
                StartCoroutine(ApagarTodo());
                StartCoroutine(TeleportJugador());
            }
        }
    }


    public void TPJugador()
    {
        Jugador.transform.position = new Vector3(referenciaTP.transform.position.x, referenciaTP.transform.position.y, referenciaTP.transform.position.z);
    }

    private IEnumerator TeleportJugador()
    {
        yield return new WaitForSeconds(1);
        TPJugador();
    }

    private IEnumerator ApagarTodo()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(referenciaTP);
        Destroy(playableObject);
        Destroy(cinematicaPortal);
    }
}
