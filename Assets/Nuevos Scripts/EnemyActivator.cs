using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [SerializeField] private GameObject enemigoVentanaRoot;
    [SerializeField] private GameObject spawnerPerseguidorRoot;

    // Activa/desactiva los roots de enemigos
    public void Activar(bool activo)
    {
        if (enemigoVentanaRoot != null) enemigoVentanaRoot.SetActive(activo);
        else Debug.LogWarning($"EnemyActivator [{name}] no tiene enemigoVentanaRoot asignado.");

        if (spawnerPerseguidorRoot != null) spawnerPerseguidorRoot.SetActive(activo);
        else Debug.LogWarning($"EnemyActivator [{name}] no tiene spawnerPerseguidorRoot asignado.");
    }

    // Llamar cuando el jugador sale de la habitación (opcional)
    public void OnPlayerLeft()
    {
        // Ejemplo: pausar spawners o cambiar estado de IA
        // Aquí solo dejamos un log; personaliza según tu IA
        Debug.Log($"EnemyActivator [{name}] OnPlayerLeft llamado.");
    }
}
