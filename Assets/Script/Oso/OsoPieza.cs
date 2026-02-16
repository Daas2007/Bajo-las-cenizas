using UnityEngine;

public class OsoPieza : MonoBehaviour
{
    [SerializeField] private OsoManager osoManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            osoManager.RecogerPieza(this);
            gameObject.SetActive(false); // pieza recogida
        }
    }
}
