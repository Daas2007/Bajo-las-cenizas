using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneTrigger : MonoBehaviour
{
    public enum TipoTrigger
    {
        ActivarMuro,   // Trigger colocado ANTES de entrar al cuarto
        CerrarPuerta   // Trigger colocado DENTRO del cuarto
    }

    [SerializeField] private string idHabitacion = "Habitacion1";
    [SerializeField] private TipoTrigger tipo = TipoTrigger.ActivarMuro;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (string.IsNullOrEmpty(idHabitacion)) return;

        switch (tipo)
        {
            case TipoTrigger.ActivarMuro:
                if (PlayerPrefs.GetInt("TieneCristal", 0) == 1)
                {
                    LevelGateManager.Instancia?.DesactivarMuroRetorno(idHabitacion);
                }
                else
                {
                    LevelGateManager.Instancia?.ActivarMuroRetorno(idHabitacion);
                }
                break;

            case TipoTrigger.CerrarPuerta:
                LevelGateManager.Instancia?.CerrarPuertaLobby(idHabitacion);
                LevelGateManager.Instancia?.EntrarHabitacion(idHabitacion);

                // 👇 Si el jugador tiene cristal, cerrar rápido la puerta
                PuertaInteractuable puerta = Object.FindFirstObjectByType<PuertaInteractuable>();
                if (puerta != null)
                {
                    puerta.CerrarSiCristal();
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Al salir del trigger con cristal → activar enemigo más rápido
        if (tipo == TipoTrigger.ActivarMuro && PlayerPrefs.GetInt("TieneCristal", 0) == 1)
        {
            EnemigoPerseguidor enemigo = Object.FindFirstObjectByType<EnemigoPerseguidor>();
            if (enemigo != null)
            {
                enemigo.SetVelocidadExtra();
            }
        }
    }
}

