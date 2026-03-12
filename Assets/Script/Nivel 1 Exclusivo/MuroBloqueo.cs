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

    public void PonerMuro()
    {
        gameObject.SetActive(true);
    }

    public void QuitarMuro()
    {
        gameObject.SetActive(false);
        Debug.Log($"[MuroBloqueo] QuitarMuro: muro desactivado ({name})");
    }
}
