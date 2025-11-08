using Mono.Cecil.Cil;
using UnityEngine;

public class Linterna : MonoBehaviour
{
    [SerializeField] GameObject luzLinterna;
    [SerializeField] AudioClip LuzSonido;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        luzLinterna.SetActive(false);
    }
    private void Update()
    {
        InterracionLinterna();
    }
    public void InterracionLinterna()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            LuzManager();
        }
    }
    public void LuzManager()
    {
        luzLinterna.SetActive(!luzLinterna.activeSelf);
        source.PlayOneShot(LuzSonido);
    }

}
