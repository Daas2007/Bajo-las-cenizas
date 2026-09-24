using UnityEngine;
using UnityEngine.UIElements;

public class MovimientojugadorMiniJuego2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float velocidadDeMovimiento = 8.0f;

    [SerializeField] bool MirandoDerecha = true;
    float anguloDerecha = 90f;
    float anguloIzquierda = 270f;
    [SerializeField] GameObject modeloPersonaje;

    private void Awake()
    {
        MirandoDerecha = true;
    }
    private void Update()
    {
        float VelocidadX = Input.GetAxis("Horizontal")*Time.deltaTime;
        Vector3 posicion = transform.position;
        transform.position = new Vector3(posicion.x + (VelocidadX * velocidadDeMovimiento), posicion.y, posicion.z);
        
        if (VelocidadX > 0 && !MirandoDerecha)
        {
            Voltear(true);
        }
        else if(VelocidadX< 0 && MirandoDerecha)
        {
            Voltear(false);
        }


    }
    void Voltear(bool haciaDerecha)
    {
        MirandoDerecha = haciaDerecha;

        float anguloTarget = MirandoDerecha ? anguloDerecha : anguloIzquierda;
        transform.rotation = Quaternion.Euler(0f,anguloTarget, 0f);

    }

}
