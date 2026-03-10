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

    // Estado local para evitar llamadas repetidas
    private bool muroActivado = false;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void Start()
    {
        // Comprobación inicial: si el jugador ya tiene el cristal, aseguramos que el muro esté desactivado
        if (tipo == TipoTrigger.ActivarMuro && !string.IsNullOrEmpty(idHabitacion))
        {
            if (PlayerPrefs.GetInt("TieneCristal", 0) == 1)
            {
                LevelGateManager.Instancia?.DesactivarMuroRetorno(idHabitacion);
                muroActivado = false;
                Debug.Log($"[ZoneTrigger] Start: jugador ya tiene cristal -> muro desactivado para {idHabitacion}");
            }
            else
            {
                // opcional: no forzar activación en Start para no sorprender al jugador,
                // pero dejamos el estado en false para que OnTriggerEnter lo active si corresponde.
                muroActivado = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (string.IsNullOrEmpty(idHabitacion)) return;

        switch (tipo)
        {
            case TipoTrigger.ActivarMuro:
                // Si el jugador tiene cristal → desactivar muro
                if (PlayerPrefs.GetInt("TieneCristal", 0) == 1)
                {
                    LevelGateManager.Instancia?.DesactivarMuroRetorno(idHabitacion);
                    muroActivado = false;
                    Debug.Log($"[ZoneTrigger] Jugador tiene cristal -> muro desactivado en {idHabitacion}");
                }
                else
                {
                    // Si no tiene cristal → activar muro
                    if (!muroActivado)
                    {
                        LevelGateManager.Instancia?.ActivarMuroRetorno(idHabitacion);
                        muroActivado = true;
                        Debug.Log($"[ZoneTrigger] Jugador NO tiene cristal -> muro activado en {idHabitacion}");
                    }
                }
                break;

            case TipoTrigger.CerrarPuerta:
                LevelGateManager.Instancia?.CerrarPuertaLobby(idHabitacion);
                LevelGateManager.Instancia?.EntrarHabitacion(idHabitacion);

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

    // -----------------------
    // Método público para notificar que el jugador recogió el cristal.
    // Llamar desde el script que maneja la recolección del cristal (o desde GameManager).
    // -----------------------
    public void NotificarCristalRecogido()
    {
        // Guardar en PlayerPrefs por compatibilidad con el resto del sistema
        PlayerPrefs.SetInt("TieneCristal", 1);
        PlayerPrefs.Save();

        // Si este trigger controla el muro de la habitación, desactivarlo
        if (tipo == TipoTrigger.ActivarMuro && !string.IsNullOrEmpty(idHabitacion))
        {
            LevelGateManager.Instancia?.DesactivarMuroRetorno(idHabitacion);
            muroActivado = false;
            Debug.Log($"[ZoneTrigger] NotificarCristalRecogido: cristal recogido -> desactivar muro {idHabitacion}");
        }
    }
}