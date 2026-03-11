using UnityEngine;

public class OsoPieza : MonoBehaviour, IInteractuable
{
    [Header("Configuración de la pieza")]
    [SerializeField] public int indiceTorso; // índice que corresponde a la parte del torso

    private bool enMano = false;

    public void Interactuar()
    {
        // ✅ Se agarra como cualquier pieza normal
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
                transform.localScale = Vector3.one;
            }
        }
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);
    }
}
