using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GestorAnimacionesJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MovimientoPersonaje movimientoJugador;

    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController animatorNormal;
    [SerializeField] private RuntimeAnimatorController animatorLinterna;
    [SerializeField] private RuntimeAnimatorController animatorLinternaObjeto;

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

        bool tieneLinterna = movimientoJugador.tieneLinterna;
        bool tieneObjeto = movimientoJugador.tieneObjeto;

        // 🔹 Cambiar Animator según estado
        if (tieneLinterna && tieneObjeto)
        {
            if (animator.runtimeAnimatorController != animatorLinternaObjeto)
                animator.runtimeAnimatorController = animatorLinternaObjeto;
        }
        else if (tieneLinterna)
        {
            if (animator.runtimeAnimatorController != animatorLinterna)
                animator.runtimeAnimatorController = animatorLinterna;
        }
        else
        {
            if (animator.runtimeAnimatorController != animatorNormal)
                animator.runtimeAnimatorController = animatorNormal;
        }

        // 🔹 Actualizar velocidad para transiciones
        float velocidad = rb.linearVelocity.magnitude;
        animator.SetFloat("Velocidad", velocidad);
    }
}
