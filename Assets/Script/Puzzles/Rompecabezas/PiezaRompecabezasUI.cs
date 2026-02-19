// PiezaRompecabezasUI.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiezaRompecabezasUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuración de la pieza")]
    public RectTransform casillaObjetivo; // slot objetivo dentro de AreaGrid
    public bool puedeRotar = false;
    public int pasoRotacion = 90; // grados por rotación
    public float distanciaSnap = 40f; // px para hacer snap
    public float toleranciaRotacion = 12f; // grados de tolerancia

    RectTransform rect;
    Canvas canvasPadre;
    Vector2 posicionOriginal;
    bool colocada = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasPadre = GetComponentInParent<Canvas>();
        posicionOriginal = rect.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (colocada) return;
        rect.SetAsLastSibling(); // traer al frente mientras arrastra
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocada) return;
        Vector2 puntoLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasPadre.transform as RectTransform,
            eventData.position,
            canvasPadre.worldCamera,
            out puntoLocal);
        rect.anchoredPosition = puntoLocal;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (colocada) return;
        IntentarSnap();
    }

    public void IntentarSnap()
    {
        if (casillaObjetivo == null) return;

        float distancia = Vector2.Distance(rect.anchoredPosition, casillaObjetivo.anchoredPosition);
        float diferenciaAngulo = Mathf.Abs(Mathf.DeltaAngle(rect.eulerAngles.z, casillaObjetivo.eulerAngles.z));

        if (distancia <= distanciaSnap && diferenciaAngulo <= toleranciaRotacion)
        {
            rect.anchoredPosition = casillaObjetivo.anchoredPosition;
            rect.eulerAngles = new Vector3(0, 0, casillaObjetivo.eulerAngles.z);
            colocada = true;
            GestorRompecabezas.Instancia.PiezaColocada();
        }
        else
        {
            // volver a la posición original si no encaja
            rect.anchoredPosition = posicionOriginal;
        }
    }

    // Llamar desde UI o tecla para rotar 90 grados
    public void Rotar90()
    {
        if (!puedeRotar || colocada) return;
        rect.Rotate(0, 0, -pasoRotacion);
    }

    // Activar la pieza faltante cuando el jugador la recoge
    public void ActivarComoFaltante()
    {
        gameObject.SetActive(true);
    }

    public bool EstaColocada() => colocada;
}
