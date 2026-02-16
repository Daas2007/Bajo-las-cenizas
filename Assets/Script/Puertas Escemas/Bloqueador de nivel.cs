using UnityEngine;

public class BloqueadorDeNivel : MonoBehaviour
{
    [SerializeField] GameObject puertaBloqueada;
    [SerializeField] CristalMetaSpawner spawner;

    void Start()
    {
        if (puertaBloqueada != null) puertaBloqueada.SetActive(true);
        GameManager.Instancia.OnOsoCompleto += OnNivelCompletado; // 🔑 ahora escucha el evento correcto
    }

    void OnDestroy()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnOsoCompleto -= OnNivelCompletado; // 🔑 cambio aquí también
    }

    void OnNivelCompletado()
    {
        if (puertaBloqueada != null) puertaBloqueada.SetActive(false);
        if (spawner != null) spawner.SpawnCristal();
        LevelGateManager.Instancia?.CompletarHabitacion("Habitacion1");
    }
}
