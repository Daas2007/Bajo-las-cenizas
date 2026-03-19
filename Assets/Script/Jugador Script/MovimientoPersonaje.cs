using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Cristal Obtenido")]
    [SerializeField] bool Cristal = false;

    [Header("Referencias")]
    [SerializeField] Transform camara;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camara camaraScript;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip jadeoClip; // Sonido de jadeo

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
    private bool estabaCorriendo = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (camara == null && Camera.main != null) camara = Camera.main.transform;
        if (canvas_StaminaBar != null) canvas_StaminaBar.SetActive(false);
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
            if (canvas_StaminaBar != null) canvas_StaminaBar.SetActive(false);
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
        rb.linearVelocity = direccionPlano * VelocidadMove;

        float velocidadActual = rb.linearVelocity.magnitude;

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

        bool corriendoAhora = Input.GetKey(KeyCode.LeftShift) && moviendo && Stamina > 0f;

        if (corriendoAhora)
        {
            VelocidadMove = VelocidadBase * 1.5f;
            Stamina -= CostoCorrer * Time.deltaTime;
            if (Stamina < 0f) Stamina = 0f;
            if (recarga != null) { StopCoroutine(recarga); recarga = null; }
        }
        else
        {
            VelocidadMove = VelocidadBase;

            // Si estaba corriendo y ahora no, verificar jadeo
            if (estabaCorriendo && !corriendoAhora && Stamina <= StaminaMaxima * 0.35f)
            {
                if (audioSource != null && jadeoClip != null)
                    audioSource.PlayOneShot(jadeoClip);
            }

            if (recarga == null) recarga = StartCoroutine(RecargaStamina());
        }

        estabaCorriendo = corriendoAhora;
    }
    void ActualizarBarraStamina()
    {
        if (BarraStamina != null)
            BarraStamina.fillAmount = Stamina / StaminaMaxima;

        bool moviendo = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (canvas_StaminaBar != null)
        {
            if ((Input.GetKey(KeyCode.LeftShift) && moviendo && Stamina > 0f) || Stamina < StaminaMaxima)
                canvas_StaminaBar.SetActive(true);
            else
                canvas_StaminaBar.SetActive(false);
        }
    }
    IEnumerator RecargaStamina()
    {
        // Si la estamina llegó a 0, esperar más tiempo antes de recargar
        float delay = (Stamina <= 0f) ? 3f : 1f;
        yield return new WaitForSeconds(delay);

        while (Stamina < StaminaMaxima)
        {
            Stamina += RecargarStamina * Time.deltaTime;
            if (Stamina > StaminaMaxima) Stamina = StaminaMaxima;
            if (BarraStamina != null) BarraStamina.fillAmount = Stamina / StaminaMaxima;
            yield return null;
        }
        if (canvas_StaminaBar != null) canvas_StaminaBar.SetActive(false);
        recarga = null;
    }
    public void GuardarPartida()
    {
        SistemaGuardar.Guardar(this, GameManager.Instancia);
    }
    public void CargarPartida()
    {
        SistemaGuardar.Cargar(this, GameManager.Instancia);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cristal"))
        {
            CristalObtenido(other.gameObject);
        }
    }
    // Versión actualizada: notifica al GameManager y actualiza ZoneTriggers sin usar PlayerPrefs
    public void CristalObtenido(GameObject cristalObject = null)
    {
        if (Cristal) return;

        Cristal = true;

        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.NotifyCrystalCollected();
        }
        else
        {
            Debug.LogWarning("[MovimientoPersonaje] GameManager.Instancia es null al recoger cristal.");
        }

        // Forzar actualización inmediata en triggers (opcional, los triggers también están suscritos al evento)
        ZoneTrigger[] triggers = FindObjectsOfType<ZoneTrigger>();
        foreach (var t in triggers)
        {
            if (t != null)
                t.ForzarActualizarEstado();
        }

        if (cristalObject != null)
            Destroy(cristalObject);

        Debug.Log("[MovimientoPersonaje] Cristal recogido: notificado a GameManager y actualizados ZoneTriggers.");
    }
    public bool TieneCristal()
    {
        return Cristal;
        Debug.Log("Ya casi ganas");
    }

}