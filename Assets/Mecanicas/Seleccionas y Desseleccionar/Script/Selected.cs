using NUnit.Framework;
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
        TextoDetect.SetActive(false);
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, Mask))
        {
            Deselect();
            SelectedObject(hit.transform);
            if (hit.collider.tag == "Objeto interactivo")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.transform.GetComponent<ObjetosInteractivo>().ActivarObjeto();
                }
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distancia, Color.red);
        }
        else
        {
            Deselect();
        }
    }

    void SelectedObject(Transform transform)
    {
        // Si es un objeto nuevo, guardamos su color original primero
        if (ultimoReconocido != transform.gameObject)
        {
            MeshRenderer renderer = transform.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                colorOriginal = renderer.material.color;

                // Color verde suave con transparencia (R: 0, G: 1, B: 0, Alpha: 0.45)
                renderer.material.color = new Color(0f, 1f, 0f, 0.45f);
            }
        }

        TextoDetect.SetActive(true);
        ultimoReconocido = transform.gameObject;
    }

    void Deselect()
    {
        if (ultimoReconocido)
        {
            MeshRenderer renderer = ultimoReconocido.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                // Devolvemos el objeto exactamente a su color almacenado previamente
                renderer.material.color = colorOriginal;
            }

            TextoDetect.SetActive(false);
            ultimoReconocido = null;
        }
    }

    private void OnGUI()
    {
        Rect rect = new Rect(Screen.width / 2, Screen.height / 2, puntero.width, puntero.height);
        GUI.DrawTexture(rect, puntero);
        if (ultimoReconocido)
        {
            TextoDetect.SetActive(true);
        }
        else
        {
            TextoDetect.SetActive(false);
        }
    }
}