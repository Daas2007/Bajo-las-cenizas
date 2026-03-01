using UnityEngine;

public class GestorPuzzleFisico : MonoBehaviour
{
    public SlotPuzzle[] slots;
    public GameObject[] puertasBloqueadas;

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
            puerta.layer = LayerMask.NameToLayer("Interaccion");
        }
    }

    private void BloquearPuzzle()
    {
        foreach (var slot in slots)
        {
            if (slot.piezaActual != null)
            {
                slot.piezaActual.enabled = false; // desactiva script de pieza
            }
        }
    }
}
