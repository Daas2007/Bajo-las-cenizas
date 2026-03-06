using UnityEngine;

public class GestorPuzzleFisico : MonoBehaviour
{
    [Header("Slots del puzzle")]
    public SlotPuzzle[] slots;

    [Header("Puertas que se desbloquean")]
    public GameObject[] puertasBloqueadas; // aquí arrastras las 2 puertas del armario

    private bool puzzleCompletado = false;

    void Update()
    {
        if (!puzzleCompletado && ComprobarPuzzle())
        {
            puzzleCompletado = true;
            DesbloquearPuertas();
            BloquearPuzzle();
        }
    }

    private bool ComprobarPuzzle()
    {
        foreach (var slot in slots)
        {
            if (!slot.EstaCorrecta()) return false;
        }
        return true;
    }

    private void DesbloquearPuertas()
    {
        foreach (var puerta in puertasBloqueadas)
        {
            // Cambia el layer a "Interaccion" para que ahora puedan abrirse
            puerta.layer = LayerMask.NameToLayer("Interaccion");
        }
    }

    private void BloquearPuzzle()
    {
        foreach (var slot in slots)
        {
            if (slot.piezaActual != null)
            {
                // Desactiva el script de la pieza para que ya no se pueda mover
                slot.piezaActual.enabled = false;
            }
        }
    }
}
