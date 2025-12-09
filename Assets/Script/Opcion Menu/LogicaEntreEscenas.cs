using UnityEngine;

public class LogicaEntreEscenas : MonoBehaviour
{
    private void Awake()
    {
        panelOpciones.SetActive(false);
    }
    [SerializeField] GameObject panelOpciones;

    //public void AbrirOpciones()
    //{
    //    panelOpciones.SetActive(true);
    //}

    //public void CerrarOpciones()
    //{
    //    panelOpciones.SetActive(false);
    //}
}
