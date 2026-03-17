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
        // ✅ Si no hay pieza en el slot, no está correcto
        if (piezaActual == null) return false;

        return piezaActual.piezaID == slotID;
    }
    public void ResetSlot()
    {
        piezaActual = null;
    }
    public void ColocarPieza(PiezaPuzzle pieza)
    {
        // ✅ Solo aceptar piezas con tag Puzzle
        if (pieza == null || !pieza.CompareTag("Puzzle"))
        {
            Debug.LogWarning($"[SlotPuzzle] El objeto '{pieza?.gameObject.name}' no tiene el tag Puzzle, no se coloca en el slot {slotID}.");
            return;
        }

        piezaActual = pieza;
        pieza.transform.SetParent(transform);
        pieza.transform.position = transform.position;
        pieza.transform.rotation = transform.rotation;
        pieza.transform.localScale = Vector3.one;

        // ✅ Marcar como colocada (desactiva gravedad y física)
        pieza.MarcarColocada();

        // Si es correcta → neutralizar
        if (pieza.piezaID == slotID)
        {
            pieza.PermitirRotacionX(true);

            Collider piezaCol = pieza.GetComponent<Collider>();
            if (piezaCol != null) piezaCol.enabled = false;

            // 🔹 Neutralizar tag y layer
            pieza.gameObject.tag = "Untagged";
            pieza.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            pieza.PermitirRotacionX(false);

            // ❌ Si no es correcta, mantener collider activo
            Collider piezaCol = pieza.GetComponent<Collider>();
            if (piezaCol != null) piezaCol.enabled = true;

            // 🔹 Mantener interactuable
            pieza.gameObject.tag = "Puzzle";
        }
    }

}