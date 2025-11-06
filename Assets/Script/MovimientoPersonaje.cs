using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    public CharacterController Controlador;
    public float velocidad = 15f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical"); 
        Vector3 mover = transform.right * x + transform.forward * z;
        Controlador.Move(mover * velocidad * Time.deltaTime );
    }
}
