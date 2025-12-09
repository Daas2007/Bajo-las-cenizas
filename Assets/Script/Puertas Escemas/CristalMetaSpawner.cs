using UnityEngine;
using UnityEngine.SceneManagement;

public class CristalMetaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cristalNormalPrefab;
    [SerializeField] private GameObject cristalDañadoPrefab;
    [SerializeField] private Transform spawnPoint;

    public void SpawnCristal()
    {
        if (GameManager.Instancia.CristalDañadoNivelActual())
        {
            Instantiate(cristalDañadoPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("⚠️ Cristal dañado instanciado.");
        }
        else
        {
            Instantiate(cristalNormalPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("✨ Cristal normal instanciado.");
        }

        // 🔑 Marcar nivel como completado
        string nivelActual = SceneManager.GetActiveScene().name;
        GameManager.Instancia.MarcarNivelCompletado(nivelActual);
    }
}
