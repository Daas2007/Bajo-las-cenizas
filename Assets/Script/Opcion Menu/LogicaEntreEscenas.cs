using UnityEngine;

public class LogicaEntreEscenas : MonoBehaviour
{
    private void Awake()
    {
        var noDestruirEntreEscenas = FindObjectsOfType<LogicaEntreEscenas>();
        if (noDestruirEntreEscenas.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        panelOpciones.SetActive(false);
    }
    [SerializeField] GameObject panelOpciones;

    public void AbrirOpciones()
    {
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
    }
}
