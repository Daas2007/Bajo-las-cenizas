using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour, IInteractuable
{
    [SerializeField] string Escena;
    [SerializeField] GameObject Fade;

    public void Interactuar()
    {
        SceneManager.LoadScene(Escena);
    }
}
