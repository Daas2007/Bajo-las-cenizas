using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cristalNormalPrefab;
    [SerializeField] private GameObject cristalDañadoPrefab;
    [SerializeField] private Transform puntoSpawn;

    public void SpawnCristal()
    {
        if (GameManager.Instancia.CristalDañadoNivelActual())
            Instantiate(cristalDañadoPrefab, puntoSpawn.position, puntoSpawn.rotation);
        else
            Instantiate(cristalNormalPrefab, puntoSpawn.position, puntoSpawn.rotation);

        GameManager.Instancia.MarcarNivelCompletado("Habitacion1");
    }
}