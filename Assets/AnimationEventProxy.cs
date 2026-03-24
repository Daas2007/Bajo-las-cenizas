using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    public EnemigoVentana enemigoVentana;

    public void FinalizarAnimacionEntradaHabitacion()
    {
        if (enemigoVentana != null)
            enemigoVentana.FinalizarAnimacionEntradaHabitacion();
    }
}
