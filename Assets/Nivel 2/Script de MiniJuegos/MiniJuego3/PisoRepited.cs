using UnityEngine;

public class PisoRepited : MonoBehaviour
{
    float spriteAncho;

    private void Start()
    {
        BoxCollider2D pisoCollider = GetComponent<BoxCollider2D>();

        // Multiplicamos por el localScale.x para obtener el ancho real en el mundo
        spriteAncho = pisoCollider.size.x * transform.localScale.x;
    }

    private void Update()
    {
        if (transform.position.x < -spriteAncho)
        {
            transform.Translate(Vector2.right * (2f * spriteAncho));
            Debug.Log("Se movio el piso " + this + "2 veces a la derecha");
        }
    }
}
