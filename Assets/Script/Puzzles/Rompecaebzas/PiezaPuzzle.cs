using UnityEngine;

public class PiezaPuzzle : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public int piezaID;
    public float toleranciaRotacion = 5f;

    private bool enMano = false;
    private bool colocada = false;
    private Transform manoIzquierda;
    private SlotPuzzle slotActual;

    void Start()
    {
        // Buscar la mano izquierda en el jugador
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
            // Rotar con R mientras está en la mano
            if (Input.GetKeyDown(KeyCode.R))
                transform.Rotate(0, 0, -90);
        }
    }

    public void Interactuar()
    {
        if (colocada) return;

        if (!enMano)
        {
            // Agarrar: parentar a la mano izquierda
            enMano = true;
            transform.SetParent(manoIzquierda);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Intentar colocar en slot
            if (slotActual != null && slotActual.slotID == piezaID)
            {
                transform.SetParent(null);
                transform.position = slotActual.transform.position;
                transform.rotation = Quaternion.identity; // rotación correcta
                slotActual.piezaActual = this;
                colocada = true;
                enMano = false;
            }
            else
            {
                // Si no hay slot correcto, soltar al suelo
                Soltar();
            }
        }
    }
    public void MarcarColocada()
    {
        colocada = true;
        enMano = false;
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);
        // Opcional: dejar caer al suelo
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
        float rotZ = transform.rotation.eulerAngles.z;
        return Mathf.Abs(rotZ) < toleranciaRotacion;
    }
}
