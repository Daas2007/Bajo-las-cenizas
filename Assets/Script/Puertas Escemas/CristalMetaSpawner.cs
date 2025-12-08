using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [SerializeField] GameObject cristalNormalPrefab;
    [SerializeField] GameObject cristalDañadoPrefab;
    [SerializeField] Transform spawnPoint;

    public void SpawnCristal()
    {
        if (GameManager.Instancia.CristalDañadoNivelActual())
        {
            Instantiate(cristalDañadoPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Cristal dañado.");
        }
        else
        {
            Instantiate(cristalNormalPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Cristal normal.");
        }
    }
}

