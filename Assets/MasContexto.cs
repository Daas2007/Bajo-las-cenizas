using UnityEngine;

public class MasContexto : MonoBehaviour
{
    [Header("Dialogo")]
    [SerializeField] private Dialogo dialogo; // referencia al sistema de diálogo

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 🔹 Activar el diálogo
            if (dialogo != null)
            {
                dialogo.ResetHaHablado();
                dialogo.IniciarDialogo();
            }
            else
            {
                Debug.LogWarning("⚠️ No se asignó un diálogo en el Inspector.");
            }

            // 🔹 Desactivar el trigger para que no se repita
            gameObject.SetActive(false);
        }
    }
}
