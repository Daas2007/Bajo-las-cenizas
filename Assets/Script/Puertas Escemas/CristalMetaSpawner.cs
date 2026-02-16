using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cristalNormalPrefab;
    [SerializeField] private GameObject cristalDañadoPrefab;
    [SerializeField] private Transform puntoSpawn;

    public void SpawnCristal()
    {
        if (puntoSpawn == null)
        {
            Debug.LogError("⚠️ No se asignó el punto de spawn.");
            return;
        }

        GameObject cristalPrefab = GameManager.Instancia.CristalDañado()
            ? cristalDañadoPrefab
            : cristalNormalPrefab;

        if (cristalPrefab != null)
        {
            Instantiate(cristalPrefab, puntoSpawn.position, puntoSpawn.rotation);
            Debug.Log($"Se generó el {(GameManager.Instancia.CristalDañado() ? "cristal dañado" : "cristal normal")}.");
        }
    }
}
