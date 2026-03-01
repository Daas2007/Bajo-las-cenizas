using UnityEngine;

public class PiezaPuzzle : MonoBehaviour
{
    public int piezaID;
    private bool colocada = false;

    private void OnMouseDrag()
    {
        if (colocada) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    private void OnMouseDown()
    {
        if (!colocada)
        {
            // Permitir rotación mientras se sostiene
            if (Input.GetKey(KeyCode.R))
                transform.Rotate(0, 0, -90);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SlotPuzzle slot = other.GetComponent<SlotPuzzle>();
        if (slot != null && slot.slotID == piezaID)
        {
            // Colocar en slot
            transform.position = slot.transform.position;
            slot.piezaActual = this;
            colocada = true;
        }
    }
}
