using UnityEngine;

public class SonidoPasos : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] pasos;

    public void Footstep()
    {
        if (audioSource != null && pasos.Length > 0)
        {
            int index = Random.Range(0, pasos.Length);
            audioSource.PlayOneShot(pasos[index]);
        }
    }
}
