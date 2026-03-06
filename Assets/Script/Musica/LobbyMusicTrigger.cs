using UnityEngine;

public class LobbyMusicTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private AudioSource musicaGeneral;
    [SerializeField] private AudioSource musicaLobby;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apagar música general y encender la del lobby
            if (musicaGeneral.isPlaying) musicaGeneral.Stop();
            if (!musicaLobby.isPlaying) musicaLobby.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apagar música del lobby y volver a la general
            if (musicaLobby.isPlaying) musicaLobby.Stop();
            if (!musicaGeneral.isPlaying) musicaGeneral.Play();
        }
    }
}
