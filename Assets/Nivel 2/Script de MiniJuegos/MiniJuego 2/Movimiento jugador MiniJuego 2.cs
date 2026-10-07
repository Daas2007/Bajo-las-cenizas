using Unity.VisualScripting;
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

    [SerializeField] private GameObject Camara;

    // Nueva variable para controlar si el movimiento está permitido
    [SerializeField] private bool puedeMoverse = true;

    private void Awake()
    {
        MirandoDerecha = true;
        puedeMoverse = true;
    }

    private void Update()
    {
        // Si no puede moverse, salimos del Update para que no lea los inputs ni mueva la cámara
        if (!puedeMoverse) return;

        float VelocidadX = Input.GetAxis("Horizontal") * Time.deltaTime;
        Vector3 posicion = transform.position;
        transform.position = new Vector3(posicion.x + (VelocidadX * velocidadDeMovimiento), posicion.y, posicion.z);

        Vector3 posicionCamare = Camara.transform.position;
        Camara.transform.position = new Vector3(posicionCamare.x + (VelocidadX * velocidadDeMovimiento), posicionCamare.y, posicionCamare.z);

        if (VelocidadX > 0 && !MirandoDerecha)
        {
            Voltear(true);
        }
        else if (VelocidadX < 0 && MirandoDerecha)
        {
            Voltear(false);
        }
    }

    void Voltear(bool haciaDerecha)
    {
        MirandoDerecha = haciaDerecha;

        float anguloTarget = MirandoDerecha ? anguloDerecha : anguloIzquierda;
        transform.rotation = Quaternion.Euler(-90f, anguloTarget, 0f);
    }

    // Método para activar o desactivar el movimiento desde otro script
    public void SetMovimientoHabilitado(bool habilitado)
    {
        puedeMoverse = habilitado;
    }
}