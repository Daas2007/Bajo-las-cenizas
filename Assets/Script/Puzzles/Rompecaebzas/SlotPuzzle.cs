using UnityEngine;

public class SlotPuzzle : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public int slotID;
    public PiezaPuzzle piezaActual;

    [Header("Punto de colocación")]
    public Transform puntoColocacion; // Empty en la posición exacta

    public void Interactuar()
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) return;

        InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
        if (interaccion == null) return;

        Transform manoIzquierda = interaccion.GetManoIzquierda();
        if (manoIzquierda.childCount == 0) return;

        PiezaPuzzle pieza = manoIzquierda.GetChild(0).GetComponent<PiezaPuzzle>();
        if (pieza == null) return;

        if (pieza.piezaID == slotID)
        {
            // ✅ Colocar en el punto de colocación
            pieza.transform.SetParent(puntoColocacion);
            pieza.transform.localPosition = Vector3.zero;
            pieza.transform.localRotation = Quaternion.identity;

            piezaActual = pieza;
            pieza.MarcarColocada();
        }
        else
        {
            pieza.Soltar();
        }
    }

    public bool EstaCorrecta()
    {
        if (piezaActual == null) return false;
        return piezaActual.piezaID == slotID &&
               Mathf.Approximately(piezaActual.transform.localRotation.eulerAngles.z, 0f);
    }

    public void ResetSlot()
    {
        piezaActual = null;
    }
}
