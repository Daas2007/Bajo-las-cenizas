using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorDialogo : MonoBehaviour
{
    [SerializeField] TMP_Text textoPersonaje;
    [SerializeField] GameObject panelOpciones;
    [SerializeField] Button botonOpcionPrefab;

    private NuevoDialogo dialogoActual;

    public void MostrarDialogo(NuevoDialogo dialogo)
    {
        dialogoActual = dialogo;
        textoPersonaje.text = dialogo.textoPersonaje;

        // Limpia opciones anteriores
        foreach (Transform child in panelOpciones.transform)
        {
            Destroy(child.gameObject);
        }

        // Crea botones para cada opción
        foreach (var opcion in dialogo.opciones)
        {
            Button nuevoBoton = Instantiate(botonOpcionPrefab, panelOpciones.transform);
            nuevoBoton.GetComponentInChildren<TMP_Text>().text = opcion.textoOpcion;
            nuevoBoton.onClick.AddListener(() => SeleccionarOpcion(opcion));
        }
    }

    void SeleccionarOpcion(OpcionDialogo opcion)
    {
        Debug.Log("Jugador eligió: " + opcion.textoOpcion);
        // Aquí decides qué pasa: cargar otro diálogo, cambiar escena, modificar variables, etc.
    }
}

