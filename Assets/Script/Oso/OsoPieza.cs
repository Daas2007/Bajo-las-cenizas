using UnityEngine;

public class OsoPieza : MonoBehaviour, IInteractuable
{
    [Header("Configuración de la pieza")]
    [SerializeField] private OsoManager osoManager;
    [SerializeField] public int indiceTorso; // índice que corresponde a la pieza en el torso

    public void Interactuar()
    {
        // ✅ Al interactuar, colocar en el torso
        if (osoManager != null)
        {
            osoManager.ColocarPieza(this);
        }

        // ✅ Destruir la pieza que estaba en la mano
        Destroy(gameObject);

        Debug.Log("🧩 Pieza del oso colocada en el torso.");
    }
}


