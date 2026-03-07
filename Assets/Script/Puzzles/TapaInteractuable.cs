using UnityEngine;

public class TapaInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    [SerializeField] private Transform pivote;
    [SerializeField] private float anguloApertura = -90f;
    [SerializeField] private float velocidad = 2f;

    private bool abierta = false;
    private bool habilitada = false;

    public void HabilitarInteraccion()
    {
        habilitada = true;

        int layerIndex = LayerMask.NameToLayer("Interaccion");
        if (layerIndex == -1)
        {
            Debug.LogWarning("[TapaInteractuable] Layer 'Interaccion' no existe. Asegúrate de crearla en Tags and Layers.");
        }
        else
        {
            gameObject.layer = layerIndex;
            Debug.Log("[TapaInteractuable] Interacción habilitada y layer asignada.");
        }
    }

    public void Interactuar()
    {
        if (!habilitada)
        {
            Debug.Log("[TapaInteractuable] Interacción no habilitada aún.");
            return;
        }

        if (!abierta)
        {
            StopAllCoroutines();
            StartCoroutine(AbrirTapa());
            abierta = true;
        }
    }

    private System.Collections.IEnumerator AbrirTapa()
    {
        if (pivote == null)
        {
            Debug.LogWarning("[TapaInteractuable] pivote no asignado.");
            yield break;
        }

        // Usamos localRotation y rotamos en Z
        Quaternion rotInicial = pivote.localRotation;
        Quaternion rotFinal = rotInicial * Quaternion.Euler(0f, 0f, anguloApertura);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;
            pivote.localRotation = Quaternion.Slerp(rotInicial, rotFinal, t);
            yield return null;
        }

        pivote.localRotation = rotFinal;

        // Una vez abierta, quitar interacción
        gameObject.layer = LayerMask.NameToLayer("Default");
        habilitada = false;
        Debug.Log("✅ Tapa abierta y desactivada la interacción.");
    }
}
