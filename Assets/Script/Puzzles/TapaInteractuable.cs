using UnityEngine;

public class TapaInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    [SerializeField] private Transform pivote; // Empty Object que rota
    [SerializeField] private float anguloApertura = -90f; // ángulo de apertura
    [SerializeField] private float velocidad = 2f;        // velocidad de rotación

    private bool abierta = false;
    private bool habilitada = false;

    // Método para habilitar la interacción cuando el candado se resuelva
    public void HabilitarInteraccion()
    {
        habilitada = true;
        gameObject.layer = LayerMask.NameToLayer("Interaccion");
    }

    public void Interactuar()
    {
        if (!habilitada) return; // no se puede interactuar si el candado no está resuelto

        if (!abierta)
        {
            StopAllCoroutines();
            StartCoroutine(AbrirTapa());
            abierta = true;
        }
    }

    private System.Collections.IEnumerator AbrirTapa()
    {
        Quaternion rotInicial = pivote.rotation;
        Quaternion rotFinal = rotInicial * Quaternion.Euler(0, anguloApertura, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * velocidad;
            pivote.rotation = Quaternion.Slerp(rotInicial, rotFinal, t);
            yield return null;
        }

        // 🔧 Una vez abierta, quitar interacción
        gameObject.layer = LayerMask.NameToLayer("Default");
        habilitada = false;
        Debug.Log("✅ Tapa abierta y desactivada la interacción.");
    }
}
