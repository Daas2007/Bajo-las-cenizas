using Unity.VisualScripting;
using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform camara;
    [SerializeField] CharacterController controlador;

    [Header("Configuracion de velocidad Player")]
    [SerializeField] bool UsarGetAxisRaw = false; //Movimiento Progresivo (True) || Movimiento inmediato (False)
    [SerializeField] float VelocidadMove = 5f;
    [SerializeField] float VelocidadBase; //Guarda la velocidad base del Player

    [Header("Gravedad")]
    [SerializeField] Vector3 velocidadVertical;
    [SerializeField] float gravedad = -9f;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();
        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }
    }
    void Start()
    {
        VelocidadBase = VelocidadMove;
    }


    void Update()
    {
        JugadorCorrer();
        JugadorCaminando();
        AplicarGravedad();
    }
    public void JugadorCaminando()
    {
        //GetAxisRaw = Movimiento inmediaro(Pasa de 0 a 1 instantaneamente) || GetAxis = Movimiento creciente (Pasa por 0.x hasta 1)
        float ValorHorizontalMove = UsarGetAxisRaw ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float ValorVerticalMove = UsarGetAxisRaw ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionPlano = (derechaCamara * ValorHorizontalMove + adelanteCamara * ValorVerticalMove);

        if (direccionPlano.sqrMagnitude > 0.001f)
        {
            direccionPlano.Normalize();
        }
        Vector3 desplazamientoXY = direccionPlano * (VelocidadMove * Time.deltaTime);
        controlador.Move(desplazamientoXY);
    }
    public void JugadorCorrer()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            VelocidadMove = VelocidadBase * 1.5f;
            Debug.Log("Velocidad actual = " + VelocidadMove);
        }
        else
        {
            VelocidadMove = VelocidadBase;
            Debug.Log("Velocidad actual =" + VelocidadMove);
        }
    }
    public void AplicarGravedad()
    {
        velocidadVertical.y += gravedad * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);
        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }
    }
}

