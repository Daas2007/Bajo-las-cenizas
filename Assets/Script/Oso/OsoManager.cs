using UnityEngine;

public class OsoManager : MonoBehaviour
{
    [SerializeField] private GameObject osoCompleto;
    [SerializeField] private CristalMetaSpawner cristalSpawner;

    public void RecogerPieza(OsoPieza pieza)
    {
        GameManager.Instancia.RecogerPieza();

        if (GameManager.Instancia.osoCompleto)
        {
            if (osoCompleto != null) osoCompleto.SetActive(true);
            if (cristalSpawner != null) cristalSpawner.SpawnCristal();
        }
    }
}
