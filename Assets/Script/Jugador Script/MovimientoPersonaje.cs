using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform camara;
    [SerializeField] CharacterController controlador;

    [Header("Configuracion de velocidad Player")]
    [SerializeField] bool UsarGetAxisRaw = true; //Movimiento Progresivo (True) || Movimiento inmediato (False)
    [SerializeField] float VelocidadMove = 5f;
    [SerializeField] float VelocidadBase; //Guarda la velocidad base del Player

    [Header("Gravedad")]
    [SerializeField] Vector3 velocidadVertical;
    [SerializeField] float gravedad = -9f;

    [Header("Stamina")]
    [SerializeField] GameObject canvas_StaminaBar;
    [SerializeField] Image BarraStamina;
    [SerializeField] float Stamina;
    [SerializeField] float StaminaMaxima;
    [SerializeField] float CostoCorrer;
    [SerializeField] float RecargarStamina;

    private Coroutine recarga;

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
        canvas_StaminaBar.SetActive(false);
        Stamina = StaminaMaxima;
        VelocidadBase = VelocidadMove;
    }
    void Update()
    {
        JugadorCorrer();
        JugadorCaminando();
        AplicarGravedad();
        if (Stamina < StaminaMaxima)
        {
            canvas_StaminaBar.SetActive(true);
        }
        else
        {
            // Oculta la barra cuando está llena y no estás corriendo
            if (!Input.GetKey(KeyCode.LeftShift))
                canvas_StaminaBar.SetActive(false);
        }
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
        if (Input.GetKey(KeyCode.LeftShift) && Stamina > 0)
        {
            canvas_StaminaBar.SetActive(true);
            VelocidadMove = VelocidadBase * 1.5f;

            //Consumo de Stamina
            Stamina -= CostoCorrer * Time.deltaTime;           
            if (Stamina <= 0) Stamina = 0;

            //Actualiza la barra de Stamina
            BarraStamina.fillAmount = Stamina / StaminaMaxima;
            //Reiniciar || Activar corrutina de recarga
            if (recarga != null) StopCoroutine(recarga);
            recarga = null;

            Debug.Log("Velocidad actual = " + VelocidadMove);           
        }
        else
        {
            VelocidadMove = VelocidadBase;

            if (recarga == null)
            {
                recarga = StartCoroutine (RecargaStamina());
            }
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
    private IEnumerator RecargaStamina()
    {
        yield return new WaitForSeconds(1f);
        while (Stamina < StaminaMaxima)
        {
            Stamina += RecargarStamina / 10f;
            if (Stamina >= StaminaMaxima) Stamina = StaminaMaxima;
            BarraStamina.fillAmount = Stamina / StaminaMaxima;
            yield return null;
        }
        canvas_StaminaBar.SetActive(false);
        recarga = null;
    }
}

