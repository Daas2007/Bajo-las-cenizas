using UnityEngine;

public class BloqueadorDeNivel : MonoBehaviour
{
    [SerializeField] GameObject puertaBloqueada;
    [SerializeField] CristalMetaSpawner spawner;

    void Start()
    {
        if (puertaBloqueada != null) puertaBloqueada.SetActive(true);
        GameManager.Instancia.OnNivelCompletado += OnNivelCompletado;
    }

    void OnDestroy()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnNivelCompletado -= OnNivelCompletado;
    }

    void OnNivelCompletado()
    {
        if (puertaBloqueada != null) puertaBloqueada.SetActive(false);
        if (spawner != null) spawner.SpawnCristal();
        LevelGateManager.Instancia?.CompletarHabitacion("Habitacion1");
    }
}