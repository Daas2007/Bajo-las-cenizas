using UnityEngine;

public class OsoPieza : MonoBehaviour, IInteractuable
{
    [Header("Configuración de la pieza")]
    [SerializeField] public int indiceTorso;

    private bool enMano = false;
    private Rigidbody rb;
    private Collider col;
    private int capaOriginal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        capaOriginal = gameObject.layer;
    }

    public void Interactuar()
    {
        if (!enMano)
        {
            Transform manoIzquierda = GameObject.FindWithTag("Player")
                .GetComponent<InteraccionJugador>()
                .GetManoIzquierda();

            if (manoIzquierda.childCount == 0)
            {
                enMano = true;
                transform.SetParent(manoIzquierda);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                // ✅ Desactivar físicas y colisiones mientras está en la mano
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                if (col != null)
                {
                    col.enabled = false;
                }

                gameObject.layer = LayerMask.NameToLayer("EnMano");
            }
        }
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);

        // ✅ Reactivar físicas y colisiones al soltar
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        if (col != null)
        {
            col.enabled = true;
        }

        gameObject.layer = capaOriginal;
    }
}
