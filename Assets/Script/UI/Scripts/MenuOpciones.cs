using UnityEngine;

public class MenuOpciones : MonoBehaviour
{
    [SerializeField] GameObject PanelOpciones;

    public void CerrarPanel()
    {
        PanelOpciones.SetActive(false);
    }
}
