using UnityEngine;

public class TriggerCerrarPuerta : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PuertaInteractuable puerta;        // Script de la puerta
    [SerializeField] private GameObject enemigo;                // Enemigo a desactivar
    [SerializeField] private VerificadorGanar verificadorGanar; // Script que maneja el fade y cambio de escena

    private void OnTriggerEnter(Collider other)
    {
        // ✅ Si el que entra es el enemigo perseguidor
        if (other.CompareTag("EnemigoPerseguidor"))
        {
            // 1. Cerrar engranaje usando el método público
            if (puerta != null)
            {
                puerta.ForzarCerrarEngranaje();
                Debug.Log("⚙️ Engranaje de la puerta cerrado por entrada del enemigo.");
            }

            // 2. Desactivar enemigo
            if (enemigo != null)
            {
                enemigo.SetActive(false);
                Debug.Log("❌ Enemigo desactivado.");
            }

            // 3. Llamar al fade y volver al menú
            if (verificadorGanar != null)
            {
                verificadorGanar.IrAlMenu("MainMenu");
                Debug.Log("🎬 Fade iniciado y regreso al menú principal.");
            }
        }
    }
}
