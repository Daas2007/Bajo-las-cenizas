using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GestorAnimacionesJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MovimientoPersonaje movimientoJugador;

    private Animator animator;
    private Rigidbody rb;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (movimientoJugador == null)
            movimientoJugador = GetComponent<MovimientoPersonaje>();
    }

    void Update()
    {
        if (animator == null || movimientoJugador == null) return;

        // 🔹 Flags globales desde MovimientoPersonaje
        bool tieneLinterna = movimientoJugador.tieneLinterna;
        bool tieneObjeto = movimientoJugador.tieneObjeto;

        // ✅ Actualizar parámetros en Animator (para transiciones automáticas si las configuras)
        animator.SetBool("TieneLinterna", tieneLinterna);
        animator.SetBool("TieneObjeto", tieneObjeto);

        // 🔹 Selección de animación según estado
        float velocidad = rb.linearVelocity.magnitude;

        if (velocidad < 0.1f)
        {
            // IDLE
            if (tieneLinterna && tieneObjeto)
                animator.Play("IdleConLinternaYObjeto");
            else if (tieneLinterna)
                animator.Play("IdleConLinterna");
            else
                animator.Play("Idle");
        }
        else if (velocidad > 0.1f && velocidad < movimientoJugador.VelocidadBase * 1.2f)
        {
            // CAMINAR
            if (tieneLinterna && tieneObjeto)
                animator.Play("CaminarConLinternaYObjeto");
            else if (tieneLinterna)
                animator.Play("CaminarConLinterna");
            else
                animator.Play("Caminar");
        }
        else if (velocidad >= movimientoJugador.VelocidadBase * 1.2f)
        {
            // CORRER
            if (tieneLinterna && tieneObjeto)
                animator.Play("CorrerConLinternaYObjeto");
            else if (tieneLinterna)
                animator.Play("CorrerConLinterna");
            else
                animator.Play("Correr");
        }
    }
}
