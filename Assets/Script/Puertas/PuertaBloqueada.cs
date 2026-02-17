using UnityEngine;

public class PuertaBloqueada : MonoBehaviour, IInteractuable
{
    [SerializeField] private bool desbloqueada = false;
    [SerializeField] private GameObject textoDialogoPrefab; // UI temporal para mostrar el mensaje

    public void Interactuar()
    {
        if (!desbloqueada)
        {
            // Mostrar mensaje como si el personaje hablara
            MostrarDialogo("Está muy oscuro... será mejor buscar algo para ver antes de salir de aquí.");
        }
        else
        {
            AbrirPuerta();
        }
    }

    public void DesbloquearPuerta()
    {
        desbloqueada = true;
    }

    private void AbrirPuerta()
    {
        Debug.Log("🚪 Puerta abierta.");
        // Aquí puedes poner animación, mover el objeto, etc.
        gameObject.SetActive(false);
    }

    private void MostrarDialogo(string mensaje)
    {
        if (textoDialogoPrefab != null)
        {
            GameObject texto = Instantiate(textoDialogoPrefab, FindObjectOfType<Canvas>().transform);
            TMPro.TMP_Text tmp = texto.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmp != null) tmp.text = mensaje;

            Destroy(texto, 3f); // se destruye el mensaje después de 3 segundos
        }
    }
}

