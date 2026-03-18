using UnityEngine;

public class LlaveInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Referencia al candado")]
    [SerializeField] private CandadoPuerta candado;

    private bool recogida = false;

    public void Interactuar()
    {
        if (!recogida)
        {
            recogida = true;
            gameObject.SetActive(false); // 🔹 Desaparece la llave al recogerla

            if (candado != null)
            {
                candado.UsarLlave();
            }

            Debug.Log("🗝️ Llave recogida y usada en el candado.");
        }
    }
}
