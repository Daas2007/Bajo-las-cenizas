using UnityEngine;

public class SlotPuzzle : MonoBehaviour
{
    public int slotID; // ID esperado
    public PiezaPuzzle piezaActual;

    public bool EstaCorrecta()
    {
        if (piezaActual == null) return false;
        return piezaActual.piezaID == slotID && Mathf.Approximately(piezaActual.transform.rotation.eulerAngles.z, 0f);
    }
}
