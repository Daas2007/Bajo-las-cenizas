using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Referencias Sistema Guardado")]
    [SerializeField] GameObject enemigo;
    [SerializeField] public bool tieneLinterna;

    [Header("Referencias")]
    [SerializeField] Transform camara;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camara camaraScript;
    [SerializeField] Animator animator; // 🔑 referencia al Animator

    [Header("Configuración de velocidad Player")]
    [SerializeField] bool UsarGetAxisRaw = true;
    [SerializeField] float VelocidadMove = 5f;
    [SerializeField] float VelocidadBase;

    [Header("Stamina")]
    [SerializeField] GameObject canvas_StaminaBar;
    [SerializeField] Image BarraStamina;
    [SerializeField] float StaminaMaxima = 100f;
    [SerializeField] float Stamina;
    [SerializeField] float CostoCorrer = 15f;
    [SerializeField] float RecargarStamina = 10f;

    private Coroutine recarga;

    void Awake()
    {
        tieneLinterna = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (camara == null && Camera.main != null) camara = Camera.main.transform;
        canvas_StaminaBar.SetActive(false);
    }

    void Start()
    {
        Stamina = StaminaMaxima;
        VelocidadBase = VelocidadMove;
    }

    void Update()
    {
        if (Time.timeScale == 1f)
        {
            JugadorCorrer();
            ActualizarBarraStamina();
        }
        else
        {
            canvas_StaminaBar.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 1f)
            JugadorCaminandoRB();
    }

    void JugadorCaminandoRB()
    {
        float h = UsarGetAxisRaw ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float v = UsarGetAxisRaw ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        Vector3 adelanteCamara = camara.forward; adelanteCamara.y = 0f; adelanteCamara.Normalize();
        Vector3 derechaCamara = camara.right; derechaCamara.y = 0f; derechaCamara.Normalize();

        Vector3 direccionPlano = (derechaCamara * h + adelanteCamara * v).normalized;
        Vector3 movimiento = direccionPlano * VelocidadMove * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movimiento);

        float velocidadActual = movimiento.magnitude / Time.fixedDeltaTime;

        // 🔑 Actualizar Animator
        if (animator != null)
        {
            animator.SetFloat("Velocidad", velocidadActual);
            animator.SetBool("Corriendo", VelocidadMove > VelocidadBase);
        }

        if (camaraScript != null)
        {
            camaraScript.SetEstado(velocidadActual);
        }
    }

    void JugadorCorrer()
    {
        bool moviendo = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (Input.GetKey(KeyCode.LeftShift) && moviendo && Stamina > 0f)
        {
            VelocidadMove = VelocidadBase * 1.5f;
            Stamina -= CostoCorrer * Time.deltaTime;
            if (Stamina < 0f) Stamina = 0f;
            if (recarga != null) { StopCoroutine(recarga); recarga = null; }
        }
        else
        {
            VelocidadMove = VelocidadBase;
            if (recarga == null) recarga = StartCoroutine(RecargaStamina());
        }
    }

    void ActualizarBarraStamina()
    {
        BarraStamina.fillAmount = Stamina / StaminaMaxima;

        bool moviendo = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if ((Input.GetKey(KeyCode.LeftShift) && moviendo && Stamina > 0f) || Stamina < StaminaMaxima)
        {
            canvas_StaminaBar.SetActive(true);
        }
        else
        {
            canvas_StaminaBar.SetActive(false);
        }
    }

    IEnumerator RecargaStamina()
    {
        yield return new WaitForSeconds(1f);
        while (Stamina < StaminaMaxima)
        {
            Stamina += RecargarStamina * Time.deltaTime;
            if (Stamina > StaminaMaxima) Stamina = StaminaMaxima;
            BarraStamina.fillAmount = Stamina / StaminaMaxima;
            yield return null;
        }
        canvas_StaminaBar.SetActive(false);
        recarga = null;
    }

    public void GuardarPartida()
    {
        SistemaGuardar.Guardar(this, enemigo, tieneLinterna);
    }

    public void CargarPartida()
    {
        SistemaGuardar.Cargar(this, enemigo, ref tieneLinterna);
    }
}

