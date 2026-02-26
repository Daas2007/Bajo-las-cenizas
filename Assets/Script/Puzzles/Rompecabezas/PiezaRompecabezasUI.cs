using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiezaRompecabezasUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //---------------Configuración---------------
    [Header("Configuración de la pieza")]
    public RectTransform casillaObjetivo; // slot objetivo dentro de AreaGrid
    public bool puedeRotar = false;
    public int pasoRotacion = 90; // grados por rotación
    public float distanciaSnap = 40f; // tolerancia de distancia
    public float toleranciaRotacion = 12f; // tolerancia de rotación

    //---------------Estado interno---------------
    private RectTransform rect;
    private Canvas canvasPadre;
    private Vector2 posicionOriginal;
    private bool colocada = false;

    //---------------Inicio---------------
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasPadre = GetComponentInParent<Canvas>();
    }

    //---------------Eventos de interacción---------------
    public void OnPointerDown(PointerEventData eventData)
    {
        if (colocada) return;
        rect.SetAsLastSibling(); // traer al frente mientras arrastra
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocada) return;
        posicionOriginal = rect.anchoredPosition; // guardar posición actual antes de arrastrar
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocada) return;
        rect.anchoredPosition += eventData.delta / canvasPadre.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocada) return;
        IntentarSnap();
    }

    //---------------Validación de encaje---------------
    private void IntentarSnap()
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
            rect.anchoredPosition = posicionOriginal; // volver a la posición previa
        }
    }

    //---------------Rotación---------------
    public void Rotar90()
    {
        if (!puedeRotar || colocada) return;
        rect.Rotate(0, 0, -pasoRotacion);
    }

    //---------------Activación especial---------------
    public void ActivarComoFaltante()
    {
        gameObject.SetActive(true);
    }

    //---------------Estado---------------
    public bool EstaColocada() => colocada;
}
