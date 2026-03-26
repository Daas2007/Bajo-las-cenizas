using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [Header("Prefabs de Cristales")]
    [SerializeField] private GameObject cristalNormalPrefab;
    [SerializeField] private GameObject cristalDañadoPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform puntoSpawn;

    [Header("Debug / Test")]
    [SerializeField] private bool forzarSpawnCristal = false; // ✅ activa el spawn manual
    [SerializeField] private bool usarCristalDañado = false;  // ✅ decide si el cristal forzado es dañado

    private void Start()
    {
        // ✅ Si está activado en el Inspector, spawnea de inmediato
        if (forzarSpawnCristal)
        {
            SpawnCristal();
        }
    }

    public void SpawnCristal()
    {
        if (puntoSpawn == null)
        {
            Debug.LogError("⚠️ No se asignó el punto de spawn.");
            return;
        }

        // ✅ Si está activado el modo forzado, ignora el GameManager
        GameObject cristalPrefab;
        if (forzarSpawnCristal)
        {
            cristalPrefab = usarCristalDañado ? cristalDañadoPrefab : cristalNormalPrefab;
        }
        else
        {
            cristalPrefab = GameManager.Instancia.CristalDañado()
                ? cristalDañadoPrefab
                : cristalNormalPrefab;
        }

        if (cristalPrefab != null)
        {
            Instantiate(cristalPrefab, puntoSpawn.position, puntoSpawn.rotation);
            Debug.Log($"Se generó el {(forzarSpawnCristal ? (usarCristalDañado ? "cristal dañado (forzado)" : "cristal normal (forzado)") : (GameManager.Instancia.CristalDañado() ? "cristal dañado" : "cristal normal"))}.");
        }
    }
}
