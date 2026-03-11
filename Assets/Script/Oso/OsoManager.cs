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
        // ✅ Si el jugador tiene una pieza en la mano, colocarla en el torso
        Transform manoIzquierda = GameObject.FindWithTag("Player")
            .GetComponent<InteraccionJugador>()
            .GetManoIzquierda();

        if (manoIzquierda.childCount > 0)
        {
            OsoPieza pieza = manoIzquierda.GetChild(0).GetComponent<OsoPieza>();
            if (pieza != null)
            {
                ColocarPieza(pieza);
                Destroy(pieza.gameObject); // destruir la pieza en la mano
            }
        }
    }

    public void ColocarPieza(OsoPieza pieza)
    {
        // ✅ Activa la parte correspondiente del torso
        if (pieza.indiceTorso >= 0 && pieza.indiceTorso < piezasTorso.Length)
        {
            piezasTorso[pieza.indiceTorso].SetActive(true);
        }

        piezasColocadas++;

        // ✅ Si todas las piezas están colocadas, activar oso completo y cristal
        if (piezasColocadas >= piezasTorso.Length)
        {
            if (osoCompleto != null) osoCompleto.SetActive(true);
            if (cristalSpawner != null) cristalSpawner.SpawnCristal();
        }
    }
}
