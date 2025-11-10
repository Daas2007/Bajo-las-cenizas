using UnityEngine;

public class PausaController : MonoBehaviour
{
    [SerializeField] private GameObject menuPausaUI;
    private bool juegoPausado = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public void PausarJuego()
    {
        menuPausaUI.SetActive(true);  
        Time.timeScale = 0f;          
        juegoPausado = true;
    }

    public void ReanudarJuego()
    {
        menuPausaUI.SetActive(false); 
        Time.timeScale = 1f;          
        juegoPausado = false;
    }

    public void OpcionesMenu()
    {

        Debug.Log("Abrir menú de opciones...");
    }


    private void Start()
    {
        menuPausaUI.SetActive(false);
        Time.timeScale = 1f;
    }
}

