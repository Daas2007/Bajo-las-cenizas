using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Interaccion : MonoBehaviour
{
    LayerMask Mask;
    public float distancia = 2.5f;

    public Texture2D puntero;
    public GameObject TextoDetect;
    GameObject ultimoReconocido = null;

    // Guardamos el color original para restaurarlo después
    private Color colorOriginal;

    private void Start()
    {
        Mask = LayerMask.GetMask("Interaccion");
        if (TextoDetect != null)
        {
            TextoDetect.SetActive(false);
        }
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, Mask))
        {
            Deselect();
            SelectedObject(hit.transform);

            string tagObjeto = hit.collider.tag;

            if (tagObjeto == "Objeto interactivo")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    var objetoInteractivo = hit.collider.transform.GetComponent<ObjetosInteractivo>();
                    if (objetoInteractivo != null)
                    {
                        objetoInteractivo.ActivarObjeto();
                    }
                }
            }
            else if (tagObjeto == "Portal")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    var portal = hit.collider.transform.GetComponent<PortalController>();
                    if (portal != null)
                    {
                        portal.Interactuar();
                    }
                }
            }
            else if (tagObjeto == "PickUp")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    var linterna = hit.collider.transform.GetComponent<LinternaPickup>();
                    if (linterna != null)
                    {
                        linterna.Interactuar();
                    }
                }
            }
            else if (tagObjeto == "Consola")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    var activador = hit.collider.transform.GetComponent<ActivadorDeMinijuegos>();
                    if (activador != null)
                    {
                        activador.Interactuar();
                    }
                }
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distancia, Color.red);
        }
        else
        {
            Deselect();
        }
    }

    void SelectedObject(Transform transformObjeto)
    {
        // Si es un objeto nuevo, guardamos su color original primero
        if (ultimoReconocido != transformObjeto.gameObject)
        {
            MeshRenderer renderer = transformObjeto.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                colorOriginal = renderer.material.color;

                // Color verde suave con transparencia (R: 0, G: 1, B: 0, Alpha: 0.45)
                renderer.material.color = new Color(0f, 1f, 0f, 0.45f);
            }
        }

        if (TextoDetect != null)
        {
            TextoDetect.SetActive(true);
        }
        ultimoReconocido = transformObjeto.gameObject;
    }

    void Deselect()
    {
        if (ultimoReconocido != null)
        {
            MeshRenderer renderer = ultimoReconocido.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                // Devolvemos el objeto exactamente a su color almacenado previamente
                renderer.material.color = colorOriginal;
            }

            if (TextoDetect != null)
            {
                TextoDetect.SetActive(false);
            }
            ultimoReconocido = null;
        }
    }

    private void OnGUI()
    {
        if (puntero != null)
        {
            Rect rect = new Rect(Screen.width / 2, Screen.height / 2, puntero.width, puntero.height);
            GUI.DrawTexture(rect, puntero);
        }

        if (TextoDetect != null)
        {
            if (ultimoReconocido != null)
            {
                TextoDetect.SetActive(true);
            }
            else
            {
                TextoDetect.SetActive(false);
            }
        }
    }
}