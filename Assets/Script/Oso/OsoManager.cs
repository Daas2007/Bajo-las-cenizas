using UnityEngine;

public class OsoManager : MonoBehaviour
{
    [Header("Configuración del oso")]
    [SerializeField] private int totalPiezas = 5; // brazos, piernas, cabeza
    private int piezasRecogidas = 0;

    [Header("Objetos a activar")]
    [SerializeField] private GameObject osoCompleto; // modelo del oso armado
    [SerializeField] private CristalMetaSpawner cristalSpawner;

    public void RecogerPieza(OsoPieza pieza)
    {
        piezasRecogidas++;

        if (piezasRecogidas >= totalPiezas)
        {
            CompletarOso();
        }
    }

    private void CompletarOso()
    {
        if (osoCompleto != null) osoCompleto.SetActive(true);

        if (cristalSpawner != null) cristalSpawner.SpawnCristal();
    }
}
