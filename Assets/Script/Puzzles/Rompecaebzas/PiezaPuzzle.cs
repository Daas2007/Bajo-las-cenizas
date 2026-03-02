using UnityEngine;

public class PiezaPuzzle : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public int piezaID;
    public float toleranciaRotacion = 5f;

    private bool enMano = false;
    public bool colocada { get; private set; }
    private Transform manoIzquierda;
    private SlotPuzzle slotActual;

    void Start()
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
            if (interaccion != null)
                manoIzquierda = interaccion.GetManoIzquierda();
        }
    }

    void Update()
    {
        if (enMano && !colocada)
        {
            if (Input.GetKeyDown(KeyCode.R))
                transform.Rotate(0, 0, -90);
        }
    }

    public void Interactuar()
    {
        if (colocada) return;

        if (!enMano)
        {
            // Agarrar
            enMano = true;
            transform.SetParent(manoIzquierda);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            if (slotActual != null && slotActual.slotID == piezaID)
            {
                colocada = true;
                enMano = false;

                // ✅ Usar el punto de colocación del slot
                Transform punto = slotActual.puntoColocacion != null ? slotActual.puntoColocacion : slotActual.transform;

                transform.SetParent(punto);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                slotActual.piezaActual = this;
            }
            else
            {
                Soltar();
            }
        }
    }


    public void MarcarColocada()
    {
        colocada = true;
        enMano = false;
    }

    public void ResetColocada()
    {
        colocada = false;
        enMano = false;
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        SlotPuzzle slot = other.GetComponent<SlotPuzzle>();
        if (slot != null)
            slotActual = slot;
    }

    private void OnTriggerExit(Collider other)
    {
        SlotPuzzle slot = other.GetComponent<SlotPuzzle>();
        if (slot != null && slot == slotActual)
            slotActual = null;
    }

    public bool EstaCorrecta()
    {
        if (!colocada) return false;
        float rotZ = transform.localRotation.eulerAngles.z;
        return Mathf.Abs(rotZ) < toleranciaRotacion;
    }
}
