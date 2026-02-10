using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cristalNormalPrefab;
    [SerializeField] private GameObject cristalDañadoPrefab;
    [SerializeField] private Transform puntoSpawn;

    public void SpawnCristal()
    {
        // Usamos el nuevo método CristalDañado() del GameManager simplificado
        if (GameManager.Instancia.CristalDañado())
        {
            Instantiate(cristalDañadoPrefab, puntoSpawn.position, puntoSpawn.rotation);
            Debug.Log("Se generó el cristal dañado.");
        }
        else
        {
            Instantiate(cristalNormalPrefab, puntoSpawn.position, puntoSpawn.rotation);
            Debug.Log("Se generó el cristal normal.");
        }

        // Ya no marcamos niveles por nombre, solo confirmamos que el nivel está completado
        GameManager.Instancia.nivelCompletado = true;
    }
}
