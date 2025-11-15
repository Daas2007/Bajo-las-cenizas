using UnityEngine;

public class LogicaOpciones : MonoBehaviour
{
    public ControladorDeOpciones panelDeOpciones;

    private void Start()
    {
        panelDeOpciones = GameObject.FindGameObjectWithTag("Opciones").GetComponent <ControladorDeOpciones>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MostrarOpciones();
        }
    }
    public void MostrarOpciones()
    {
        panelDeOpciones.pantallaOpciones.SetActive(true);
    }
}
