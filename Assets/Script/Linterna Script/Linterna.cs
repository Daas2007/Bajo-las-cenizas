using Mono.Cecil.Cil;
using UnityEngine;

public class Linterna : MonoBehaviour
{
    [SerializeField] Light luzLinterna;
    [SerializeField] bool Activada = true;
    [SerializeField] bool Desactivada = false;
    [SerializeField] bool EstadoLinternaCatual;

    private void Awake()
    {
        EstadoLinternaCatual = Desactivada;
        luzLinterna.enabled = EstadoLinternaCatual;
    }
    private void Update()
    {
        InterracionLinterna();

    }
    public void InterracionLinterna()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (luzLinterna.enabled == Activada)
            {
                luzLinterna.enabled = Desactivada;
                Debug.Log("Linterna Apagada");
            }
            else if (luzLinterna.enabled == Desactivada)
            {
                luzLinterna.enabled = Activada;
                Debug.Log("Linterna Encendida");
            }
        }
    }
}
