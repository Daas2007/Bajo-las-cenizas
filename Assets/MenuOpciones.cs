using UnityEngine;

public class MenuOpciones : MonoBehaviour
{
    [Header("Paneles de origen")]
    [SerializeField] GameObject mainPanel;   // Panel del menú principal
    [SerializeField] GameObject pausePanel;  // Panel del menú de pausa

    private GameObject origen; // quién abrió opciones

    void Awake()
    {
        gameObject.SetActive(false); // arranca oculto
    }

    // Abrir desde el menú principal
    public void AbrirDesdeMain()
    {
        origen = mainPanel;
        if (mainPanel != null) mainPanel.SetActive(false);
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Abrir desde el menú de pausa
    public void AbrirDesdePausa()
    {
        origen = pausePanel;
        if (pausePanel != null) pausePanel.SetActive(false);
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Volver al menú que lo abrió
    public void Volver()
    {
        gameObject.SetActive(false);

        if (origen != null)
        {
            origen.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
