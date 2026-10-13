using UnityEngine;

public class PisoRepited : MonoBehaviour
{
    float spriteAncho;

    private void Start()
    {
        BoxCollider2D pisoCollider = GetComponent<BoxCollider2D>();
        spriteAncho = pisoCollider.size.x;
    }

    private void Update()
    {
        if (transform.position.x < -spriteAncho)
        {
            transform.Translate(Vector2.right * 2f * spriteAncho);
        }
    }
}
