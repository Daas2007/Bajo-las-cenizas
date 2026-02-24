using UnityEngine;

public class MuroBloqueo : MonoBehaviour
{
    private Vector3 posicionInicial;

    void Awake()
    {
        posicionInicial = transform.position;
    }

    public void ResetMuro()
    {
        gameObject.SetActive(true);
        transform.position = posicionInicial;
    }
}

