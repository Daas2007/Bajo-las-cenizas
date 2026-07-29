using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Cristal Obtenido")]
    public bool Cristal = false;
    [SerializeField] private GameObject verificadorGanar;

    [Header("Referencias")]
    [SerializeField] private Transform camara;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camara camaraScript;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jadeoClip;

    [Header("Configuración de Velocidad")]
    [SerializeField] private bool usarGetAxisRaw = true;
    [SerializeField] private float velocidadMove = 5f;
    public float VelocidadBase { get; private set; }

    [Header("Stamina")]
    [SerializeField] private GameObject canvasStaminaBar;
    [SerializeField] private Image barraStamina;
    [SerializeField] private float staminaMaxima = 100f;
    [SerializeField] private float costoCorrer = 15f;
    [SerializeField] private float recargarStamina = 10f;

    public float Stamina { get; private set; }

    // Control Interno de Stamina (Evita Corrutinas / GC Allocations)
    private float temporizadorRecarga;
    private bool estabaCorriendo;

    // Flags para Animator
    [HideInInspector] public bool tieneLinterna;
    [HideInInspector] public bool tieneObjeto;

    // Caché de Inputs y Físicas
    private Vector3 direccionInput;
    private bool quiereCorrer;
    private bool estaMoviendose;

    // 🔹 Optimization: Animator Hashes (Evita alloc de Strings por frame)
    private static readonly int HashVelocidad = Animator.StringToHash("Velocidad");
    private static readonly int HashCorriendo = Animator.StringToHash("Corriendo");
    private static readonly int HashLinterna = Animator.StringToHash("TieneLinterna");
    private static readonly int HashObjeto = Animator.StringToHash("TieneObjeto");

    // 🔹 Optimization: Tag Hash
    private static readonly string TagCristal = "Cristal";

    private void Awake()
    {
        if (verificadorGanar != null)
            verificadorGanar.SetActive(false);

        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        if (canvasStaminaBar != null)
            canvasStaminaBar.SetActive(false);
    }

    private void Start()
    {
        Stamina = staminaMaxima;
        VelocidadBase = velocidadMove;
    }

    private void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            if (canvasStaminaBar != null && canvasStaminaBar.activeSelf)
                canvasStaminaBar.SetActive(false);
            return;
        }

        LeerEntradas();
        GestionarStamina();
        ActualizarBarraStaminaUI();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale > 0f)
            MoverJugador();
    }

    private void LeerEntradas()
    {
        float h = usarGetAxisRaw ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float v = usarGetAxisRaw ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        // Calculamos dirección orientada a la cámara
        Vector3 adelante = camara.forward; adelante.y = 0f; adelante.Normalize();
        Vector3 derecha = camara.right; derecha.y = 0f; derecha.Normalize();

        direccionInput = (derecha * h + adelante * v).normalized;
        estaMoviendose = direccionInput.sqrMagnitude > 0.01f; // Más rápido que Vector3.magnitude

        quiereCorrer = Input.GetKey(KeyCode.LeftShift) && estaMoviendose && Stamina > 0f;
    }

    private void MoverJugador()
    {
        // Preservamos la velocidad vertical Y (gravedad, caídas)
        Vector3 velocidadObjetivo = direccionInput * velocidadMove;
        velocidadObjetivo.y = rb.linearVelocity.y;

        rb.linearVelocity = velocidadObjetivo;

        float velocidadPlano = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

        // Actualizar Animaciones con Hashes
        if (animator != null)
        {
            animator.SetFloat(HashVelocidad, velocidadPlano);
            animator.SetBool(HashCorriendo, velocidadMove > VelocidadBase);
            animator.SetBool(HashLinterna, tieneLinterna);
            animator.SetBool(HashObjeto, tieneObjeto);
        }

        if (camaraScript != null)
            camaraScript.SetEstado(velocidadPlano);
    }

    private void GestionarStamina()
    {
        if (quiereCorrer)
        {
            velocidadMove = VelocidadBase * 1.5f;
            Stamina = Mathf.Max(0f, Stamina - costoCorrer * Time.deltaTime);
            temporizadorRecarga = (Stamina <= 0f) ? 3f : 1f; // Delay dinámico
            estabaCorriendo = true;
        }
        else
        {
            velocidadMove = VelocidadBase;

            // Sonido de jadeo al agotar la energía
            if (estabaCorriendo && Stamina <= staminaMaxima * 0.35f)
            {
                if (audioSource != null && jadeoClip != null)
                    audioSource.PlayOneShot(jadeoClip);
            }
            estabaCorriendo = false;

            // Recarga limpia por tiempo (Sin Corrutinas)
            if (temporizadorRecarga > 0f)
            {
                temporizadorRecarga -= Time.deltaTime;
            }
            else if (Stamina < staminaMaxima)
            {
                Stamina = Mathf.Min(staminaMaxima, Stamina + recargarStamina * Time.deltaTime);
            }
        }
    }

    private void ActualizarBarraStaminaUI()
    {
        if (canvasStaminaBar == null) return;

        bool mostrarBarra = (quiereCorrer) || (Stamina < staminaMaxima);

        if (canvasStaminaBar.activeSelf != mostrarBarra)
            canvasStaminaBar.SetActive(mostrarBarra);

        if (mostrarBarra && barraStamina != null)
            barraStamina.fillAmount = Stamina / staminaMaxima;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagCristal))
        {
            CristalObtenido(other.gameObject);
        }
    }

    public void CristalObtenido(GameObject cristalObject = null)
    {
        if (Cristal) return;

        Cristal = true;

        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.NotifyCrystalCollected();
        }

        // 💡 RECOMENDACIÓN: Sustituir FindObjectsOfType en el futuro usando un sistema de Eventos o Registro.
        ZoneTrigger[] triggers = FindObjectsOfType<ZoneTrigger>();
        for (int i = 0; i < triggers.Length; i++)
        {
            if (triggers[i] != null)
                triggers[i].ForzarActualizarEstado();
        }

        if (cristalObject != null)
            Destroy(cristalObject);

        if (verificadorGanar != null)
            verificadorGanar.SetActive(true);
    }

    public bool TieneCristal()
    {
        if (Cristal && verificadorGanar != null && !verificadorGanar.activeSelf)
        {
            verificadorGanar.SetActive(true);
        }

        return Cristal;
    }

    public void GuardarPartida() => SistemaGuardar.Guardar(this, GameManager.Instancia);
    public void CargarPartida() => SistemaGuardar.Cargar(this, GameManager.Instancia);
}