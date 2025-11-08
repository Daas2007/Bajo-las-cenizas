using Unity.Mathematics;
using UnityEngine;

public class Camara : MonoBehaviour
{
    [SerializeField] float Sensibilidad = 100f;
    [SerializeField] Transform Player;
    [SerializeField] float rotacionHorizontal = 0f;
    [SerializeField] float rotacionVertical = 0f;
    void Start()
    {
        OcultarMouse();
    }

    void Update()
    {
        //Valores del Mouse
        float valorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float valorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        //Guardado de valor/posicion del mouse
        rotacionHorizontal += valorX;
        rotacionVertical -= valorY;

        //Limitador de giro en Eje Y
        rotacionVertical = math.clamp(rotacionVertical, -70f, 70f);

        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
        if (Player != null)
        {
            Player.Rotate(Vector3.up * valorX);
        }
        else
        {
            Debug.Log("Falta asignas Jugador en Script: Camara");
        }
    }

    public void OcultarMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
