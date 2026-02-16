using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [Header("Prefabs de cristales")]
    [SerializeField] private GameObject cristalNormalPrefab;
    [SerializeField] private GameObject cristalDañadoPrefab;

    [Header("Punto de aparición")]
    [SerializeField] private Transform puntoSpawn;

    public void SpawnCristal()
    {
        if (puntoSpawn == null)
        {
            Debug.LogError("⚠️ No se asignó el punto de spawn en el Inspector.");
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
        else
        {
            Debug.LogWarning("⚠️ No hay prefab asignado para el cristal correspondiente.");
        }
    }
}
