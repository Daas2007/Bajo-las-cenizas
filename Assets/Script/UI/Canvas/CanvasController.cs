using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] GameObject panelDialogo;
    [SerializeField] GameObject panelPista;
    [SerializeField] GameObject panelPausa;
    [SerializeField] GameObject panelOpciones;

    [Header("HUD siempre visible")]
    [SerializeField] GameObject panelHUD; // crosshair, stamina, etc.

    private GameObject panelActivo;

    void Start()
    {
        DesactivarTodos();
        if (panelHUD != null) panelHUD.SetActive(true);
    }

    public void MostrarDialogo()
    {
        DesactivarTodos();
        panelDialogo.SetActive(true);
        panelActivo = panelDialogo;
    }

    public void MostrarPista()
    {
        DesactivarTodos();
        panelPista.SetActive(true);
        panelActivo = panelPista;
    }

    public void MostrarPausa()
    {
        DesactivarTodos();
        panelPausa.SetActive(true);
        panelActivo = panelPausa;
    }

    public void MostrarOpciones()
    {
        DesactivarTodos();
        panelOpciones.SetActive(true);
        panelActivo = panelOpciones;
    }

    public void CerrarPanelActivo()
    {
        if (panelActivo != null)
        {
            panelActivo.SetActive(false);
            panelActivo = null;
        }
    }

    private void DesactivarTodos()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelPista != null) panelPista.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }
}
