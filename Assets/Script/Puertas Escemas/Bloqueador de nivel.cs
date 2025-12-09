using UnityEngine;

public class BloqueadorDeNivel : MonoBehaviour
{
    [Header("Puerta bloqueada")]
    [SerializeField]  GameObject puertaBloqueada;

    [Header("Spawner de cristal")]
    [SerializeField]  CristalMetaSpawner spawner;

    void Start()
    {
        // Al inicio, puerta cerrada
        if (puertaBloqueada != null)
            puertaBloqueada.SetActive(true);

        // Suscribirse al evento de nivel completado
        GameManager.Instancia.OnNivelCompletado += OnNivelCompletado;
    }

     void OnDestroy()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnNivelCompletado -= OnNivelCompletado;
    }

    void OnNivelCompletado()
    {
        // Desbloquear salida
        if (puertaBloqueada != null)
            puertaBloqueada.SetActive(false);

        // Instanciar cristal correcto
        if (spawner != null)
            spawner.SpawnCristal();
    }
}
