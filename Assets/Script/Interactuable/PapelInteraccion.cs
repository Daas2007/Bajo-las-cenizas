using UnityEngine;

public class PapelInteraccion : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject canvasPapel;
    public Light LuzInterracion;
    private bool yaInteractuado = false;
    private void Awake()
    {
        LuzInterracion.gameObject.SetActive(!yaInteractuado);
        canvasPapel.SetActive(false);
    }
    public void Interactuar()
    {
        if (!yaInteractuado)
        {
            yaInteractuado = true;
            LuzInterracion.gameObject.SetActive(!yaInteractuado);            
        }

        canvasPapel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CerrarCanvas()
    {
        canvasPapel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
