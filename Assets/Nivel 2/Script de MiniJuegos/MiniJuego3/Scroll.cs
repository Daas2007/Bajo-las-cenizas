using UnityEngine;

public class Scroll : MonoBehaviour
{
    [SerializeField] private float velocidadScroll = 8f;

    private void Update()
    {
        transform.Translate(Vector2.left * velocidadScroll * Time.deltaTime);
    }
}
