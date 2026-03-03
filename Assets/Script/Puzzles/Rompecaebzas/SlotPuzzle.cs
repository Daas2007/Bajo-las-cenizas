using UnityEngine;

public class SlotPuzzle : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public int slotID;
    public PiezaPuzzle piezaActual;

    private BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        // ✅ Si no tiene hijo, tag = "Colocar" y collider activo
        if (transform.childCount == 0)
        {
            gameObject.tag = "Colocar";
            if (boxCollider != null) boxCollider.enabled = true;
        }
        else
        {
            // ✅ Si tiene hijo, tag vacío y collider desactivado
            gameObject.tag = "Untagged";
            if (boxCollider != null) boxCollider.enabled = false;
        }
    }

    public void Interactuar()
    {
        // La lógica de colocar la maneja InteraccionJugador.IntentarColocar()
    }

    public bool EstaCorrecta()
    {
        if (piezaActual == null) return false;
        return piezaActual.piezaID == slotID;
    }

    public void ResetSlot()
    {
        piezaActual = null;
    }
}
