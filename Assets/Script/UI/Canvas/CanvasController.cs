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

    void Update()
    {
        // Control con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelActivo == panelDialogo || panelActivo == panelPista)
            {
                // Si estaba en diálogo o pista, cerrar y abrir pausa
                CerrarPanelActivo();
                MostrarPausa();
            }
            else if (panelActivo == panelPausa)
            {
                // Si estaba en pausa, cerrar pausa y volver al gameplay
                CerrarPanelActivo();
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (panelActivo == null)
            {
                // Si no hay panel activo, abrir pausa
                MostrarPausa();
            }
        }
    }

    public void MostrarDialogo()
    {
        DesactivarTodos();
        panelDialogo.SetActive(true);
        panelActivo = panelDialogo;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MostrarPista()
    {
        DesactivarTodos();
        panelPista.SetActive(true);
        panelActivo = panelPista;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MostrarPausa()
    {
        DesactivarTodos();
        panelPausa.SetActive(true);
        panelActivo = panelPausa;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MostrarOpciones()
    {
        DesactivarTodos();
        panelOpciones.SetActive(true);
        panelActivo = panelOpciones;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
