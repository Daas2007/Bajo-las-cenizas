using UnityEngine;

public class CristalMetaSpawner : MonoBehaviour
{
    [SerializeField] GameObject cristalPrefab;
    [SerializeField] Transform puntoSpawn;

    GameObject cristalInstanciado;

    void OnEnable()
    {
        GameManager.Instancia.OnNivelCompletado += InstanciarCristal;
    }

    void OnDisable()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnNivelCompletado -= InstanciarCristal;
    }

    void InstanciarCristal()
    {
        if (cristalInstanciado == null && cristalPrefab != null && puntoSpawn != null)
        {
            cristalInstanciado = Instantiate(cristalPrefab, puntoSpawn.position, puntoSpawn.rotation);
            Debug.Log("? Cristal meta instanciado");
        }
    }
}
