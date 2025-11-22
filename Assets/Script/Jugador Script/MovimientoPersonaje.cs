using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform camara;
    [SerializeField] Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (camara == null && Camera.main != null) camara = Camera.main.transform;
    }

    void Start()
    {
        Stamina = StaminaMaxima;
        VelocidadBase = VelocidadMove;
        canvas_StaminaBar.SetActive(false);
    }

    void Update()
    {
        JugadorCorrer();
        ActualizarBarraStamina();
    }

    void FixedUpdate()
    {
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
    }

    void JugadorCorrer()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Stamina > 0f)
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
        canvas_StaminaBar.SetActive(Stamina < StaminaMaxima || Input.GetKey(KeyCode.LeftShift));
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
}
