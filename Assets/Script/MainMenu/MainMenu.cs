using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject menuOpciones;
    [SerializeField] GameObject mainMenu;
    public void AbrirPanelDeOpciones()
    {
        mainMenu.SetActive(false);
        menuOpciones.SetActive(true);
    }
    public void AbrirPanelDeMenu()
    {
        mainMenu.SetActive(true);
        menuOpciones.SetActive(false);
    }
    public void SalirDelJuego()
    {
        Application.Quit();
    }
    public void IniciarJuego()
    {
        SceneManager.LoadScene("GameplayScene");
    }
}
