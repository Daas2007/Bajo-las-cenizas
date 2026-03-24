using UnityEngine;

public class SonidoPasos : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] pasos;

    private Animator animator;

    // Hashes de todos los estados de caminar y correr
    private int caminarHash = Animator.StringToHash("Caminar");
    private int caminarLinternaHash = Animator.StringToHash("Caminar Con Linterna");
    private int caminarObjetoHash = Animator.StringToHash("Caminar Con Objeto");

    private int correrHash = Animator.StringToHash("Correr Sin Objetos");
    private int correrLinternaHash = Animator.StringToHash("Correr Con Linterna");
    private int correrObjetoHash = Animator.StringToHash("Correr Con Objeto");

    // Flags para evitar repetir sonidos en el mismo ciclo
    private bool pasoWalk1, pasoWalk2;
    private bool pasoRun1, pasoRun2;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null || audioSource == null || pasos.Length == 0) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float t = stateInfo.normalizedTime % 1f;

        // 🔹 Estados de caminar
        if (stateInfo.shortNameHash == caminarHash ||
            stateInfo.shortNameHash == caminarLinternaHash ||
            stateInfo.shortNameHash == caminarObjetoHash)
        {
            if (t >= 0.30f && !pasoWalk1) { PlayStep(); pasoWalk1 = true; }
            if (t >= 0.53f && !pasoWalk2) { PlayStep(); pasoWalk2 = true; }

            if (t < 0.30f) pasoWalk1 = false;
            if (t < 0.53f) pasoWalk2 = false;
        }
        // 🔹 Estados de correr
        else if (stateInfo.shortNameHash == correrHash ||
                 stateInfo.shortNameHash == correrLinternaHash ||
                 stateInfo.shortNameHash == correrObjetoHash)
        {
            if (t >= 0.14f && !pasoRun1) { PlayStep(); pasoRun1 = true; }
            if (t >= 0.63f && !pasoRun2) { PlayStep(); pasoRun2 = true; }

            if (t < 0.14f) pasoRun1 = false;
            if (t < 0.63f) pasoRun2 = false;
        }
        else
        {
            // 🔹 Si no está en caminar ni correr, resetea flags
            pasoWalk1 = pasoWalk2 = pasoRun1 = pasoRun2 = false;
        }
    }

    private void PlayStep()
    {
        int index = Random.Range(0, pasos.Length);
        audioSource.PlayOneShot(pasos[index]);
    }
}
