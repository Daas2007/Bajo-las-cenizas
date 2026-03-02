using UnityEngine;

public class SlotPuzzle : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public int slotID; // ID esperado
    public PiezaPuzzle piezaActual;

    public void Interactuar()
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) return;

        InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
        if (interaccion == null) return;

        Transform manoIzquierda = interaccion.GetManoIzquierda();
        if (manoIzquierda.childCount == 0) return; // no hay nada en la mano

        PiezaPuzzle pieza = manoIzquierda.GetChild(0).GetComponent<PiezaPuzzle>();
        if (pieza == null) return;

        // Validar si la pieza corresponde a este slot
        if (pieza.piezaID == slotID)
        {
            pieza.transform.SetParent(null);
            pieza.transform.position = transform.position;
            pieza.transform.rotation = Quaternion.identity;
            piezaActual = pieza;
            pieza.MarcarColocada(); // ahora sí queda fija
        }
        else
        {
            // Si no coincide, soltar la pieza al suelo (no queda fija)
            pieza.Soltar();
        }
    }

    public bool EstaCorrecta()
    {
        if (piezaActual == null) return false;
        return piezaActual.piezaID == slotID &&
               Mathf.Approximately(piezaActual.transform.rotation.eulerAngles.z, 0f);
    }
}
