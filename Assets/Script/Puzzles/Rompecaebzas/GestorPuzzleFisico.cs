using UnityEngine;

public class GestorPuzzleFisico : MonoBehaviour
{
    [Header("Slots del puzzle")]
    public SlotPuzzle[] slots;

    [Header("Pieza del oso que se activa")]
    public GameObject piezaOso; // arrastra aquí la pieza del oso

    private bool puzzleCompletado = false;

    private void Start()
    {
        // ✅ Garantiza que arranque desactivada
        if (piezaOso != null)
            piezaOso.SetActive(false);
    }

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
            piezaOso.SetActive(true);
            Debug.Log("✅ Puzzle completado, pieza del oso activada.");
        }
    }

    private void BloquearPuzzle()
    {
        foreach (var slot in slots)
        {
            if (slot.piezaActual != null)
            {
                slot.piezaActual.enabled = false;
            }
        }
    }

    // 🔹 Reset opcional
    public void ResetPuzzle()
    {
        puzzleCompletado = false;
        if (piezaOso != null)
            piezaOso.SetActive(false);

        foreach (var slot in slots)
        {
            if (slot.piezaActual != null)
            {
                slot.piezaActual.ResetColocada();
                slot.piezaActual.enabled = true;
                slot.ResetSlot();
            }
        }
    }
}
