using UnityEngine;

public class GestorPuzzleFisico : MonoBehaviour
{
    [Header("Slots del puzzle")]
    public SlotPuzzle[] slots;

    [Header("Pieza del oso que se activa")]
    public GameObject piezaOso; // arrastra aquí la pieza del oso (déjala desactivada en el Inspector)

    private bool puzzleCompletado = false;

    void Update()
    {
        if (!puzzleCompletado && ComprobarPuzzle())
        {
            puzzleCompletado = true;
            ActivarPiezaOso();
            BloquearPuzzle();
        }
    }

    private bool ComprobarPuzzle()
    {
        // ✅ Solo devuelve true si TODOS los slots tienen la pieza correcta
        foreach (var slot in slots)
        {
            if (!slot.EstaCorrecta()) return false;
        }
        return true;
    }

    private void ActivarPiezaOso()
    {
        if (piezaOso != null && !piezaOso.activeSelf)
        {
            piezaOso.SetActive(true); // ✅ activa la pieza del oso
            Debug.Log("✅ Puzzle completado, pieza del oso activada.");
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