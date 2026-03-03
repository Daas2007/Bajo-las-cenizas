using UnityEngine;

public class OsoManager : MonoBehaviour
{
    [Header("Configuración del oso")]
    [SerializeField] private GameObject torso;             // referencia al torso del peluche
    [SerializeField] private GameObject[] piezasTorso;     // piezas desactivadas en el torso (brazos, piernas, cabeza)
    [SerializeField] private GameObject osoCompleto;       // oso completo final
    [SerializeField] private CristalMetaSpawner cristalSpawner;

    private int piezasColocadas = 0;

    public void ColocarPieza(OsoPieza pieza)
    {
        // ✅ Activar la pieza correspondiente en el torso
        if (pieza.indiceTorso >= 0 && pieza.indiceTorso < piezasTorso.Length)
        {
            piezasTorso[pieza.indiceTorso].SetActive(true);
        }

        piezasColocadas++;

        // ✅ Si todas las piezas están colocadas
        if (piezasColocadas >= piezasTorso.Length)
        {
            if (osoCompleto != null) osoCompleto.SetActive(true);
            if (cristalSpawner != null) cristalSpawner.SpawnCristal();
        }
    }
}
     
