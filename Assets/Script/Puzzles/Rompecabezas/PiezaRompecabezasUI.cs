using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiezaRompecabezasUI : MonoBehaviour,
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform casillaObjetivo;
    public float distanciaSnap = 40f;
    public float toleranciaRotacion = 12f;
    public bool puedeRotar = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 posicionInicial;
    private Quaternion rotacionInicial;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        posicionInicial = rectTransform.anchoredPosition;
        rotacionInicial = rectTransform.rotation;
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rectTransform.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (Vector2.Distance(rectTransform.anchoredPosition, casillaObjetivo.anchoredPosition) <= distanciaSnap &&
            Mathf.Abs(Quaternion.Angle(rectTransform.rotation, casillaObjetivo.rotation)) <= toleranciaRotacion)
        {
            rectTransform.anchoredPosition = casillaObjetivo.anchoredPosition;
            rectTransform.rotation = casillaObjetivo.rotation;
            GestorRompecabezas.Instancia.PiezaColocada();
        }
        else
        {
            rectTransform.anchoredPosition = posicionInicial;
            rectTransform.rotation = rotacionInicial;
        }
    }

    public bool EstaColocada()
    {
        return Vector2.Distance(rectTransform.anchoredPosition, casillaObjetivo.anchoredPosition) <= distanciaSnap &&
               Mathf.Abs(Quaternion.Angle(rectTransform.rotation, casillaObjetivo.rotation)) <= toleranciaRotacion;
    }
}
