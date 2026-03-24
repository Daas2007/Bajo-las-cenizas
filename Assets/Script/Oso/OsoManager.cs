using UnityEngine;

public class OsoManager : MonoBehaviour, IInteractuable
{
    [Header("Configuración del oso")]
    [SerializeField] private GameObject[] piezasTorso;     // partes del torso (brazos, piernas, cabeza)
    [SerializeField] private GameObject osoCompleto;       // oso completo final
    [SerializeField] private CristalMetaSpawner cristalSpawner;

    private int piezasColocadas = 0;

    public void Interactuar()
    {
        // ✅ Obtener la mano desde InteraccionJugador
        InteraccionJugador interaccion = FindObjectOfType<InteraccionJugador>();
        if (interaccion == null)
        {
            Debug.LogError("❌ No se encontró InteraccionJugador en la escena.");
            return;
        }

        Transform manoIzquierda = interaccion.GetManoIzquierda();
        if (manoIzquierda == null)
        {
            Debug.LogError("❌ La referencia de manoIzquierda está en null.");
            return;
        }

        if (manoIzquierda.childCount > 0)
        {
            OsoPieza pieza = manoIzquierda.GetChild(0).GetComponent<OsoPieza>();
            if (pieza != null)
            {
                ColocarPieza(pieza);

                pieza.Soltar();
                Destroy(pieza.gameObject);
            }
            else
            {
                Debug.Log("⚠️ El objeto en la mano no es una pieza del oso.");
            }
        }
        else
        {
            Debug.Log("⚠️ No tienes ninguna pieza del oso en la mano.");
        }
    }

    public void ColocarPieza(OsoPieza pieza)
    {
        if (pieza.indiceTorso >= 0 && pieza.indiceTorso < piezasTorso.Length)
        {
            piezasTorso[pieza.indiceTorso].SetActive(true);
            Debug.Log("✅ Pieza del oso colocada en el torso: " + pieza.indiceTorso);
        }

        piezasColocadas++;

        if (piezasColocadas >= piezasTorso.Length)
        {
            if (osoCompleto != null) osoCompleto.SetActive(true);
            if (cristalSpawner != null) cristalSpawner.SpawnCristal();

            Debug.Log("🎉 Oso completo armado. Cristal generado.");
        }
    }
}
