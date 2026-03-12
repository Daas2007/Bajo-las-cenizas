//using UnityEngine;
//using UnityEngine.UI;

///// <summary>
///// Controla dos panels UI (top y bottom) para simular un "ojo" que se cierra.
///// - Asignar topPanel y bottomPanel (RectTransform).
///// - Ajustar maxPanelHeight (en píxeles) o dejar 0 para usar Screen.height * 0.5f.
///// - usarSizeDelta = true (recomendado) modifica sizeDelta.y; false mueve anchoredPosition.
///// </summary>
//[DisallowMultipleComponent]
//public class EyeCloseEffect : MonoBehaviour
//{
//    [Header("UI panels (RectTransforms)")]
//    [SerializeField] private RectTransform topPanel;
//    [SerializeField] private RectTransform bottomPanel;

//    [Header("Configuración")]
//    [Tooltip("Altura máxima en píxeles que cubrirá cada panel cuando esté completamente cerrado. Si 0 -> Screen.height * 0.5")]
//    [SerializeField] private float maxPanelHeight = 0f;
//    [Tooltip("Tiempo (s) para suavizar la interpolación")]
//    [SerializeField] private float smoothTime = 0.25f;
//    [Tooltip("Si true modifica sizeDelta.y; si false mueve anchoredPosition")]
//    [SerializeField] private bool usarSizeDelta = true;

//    // estado interno
//    private float targetAmount = 0f; // 0 = abierto, 1 = cerrado
//    private float currentAmount = 0f;
//    private float velocity = 0f;

//    //void Start()
//    {
//        if (topPanel == null || bottomPanel == null)
//        {
//            Debug.LogError("[EyeCloseEffect] Asigna topPanel y bottomPanel en el Inspector.");
//            enabled = false;
//            return;
//        }

//        if (maxPanelHeight <= 0f)
//            maxPanelHeight = Screen.height * 0.5f;

//        ApplyAmountImmediate(0f);
//    }

//    void Update()
//    {
//        // Suavizado entre currentAmount y targetAmount
//        currentAmount = Mathf.SmoothDamp(currentAmount, targetAmount, ref velocity, Mathf.Max(0.0001f, smoothTime));
//        ApplyAmount(currentAmount);
//    }

//    /// <summary>
//    /// Establece el objetivo de cierre (0..1). Llamar desde otro script.
//    /// </summary>
//    public void SetTargetAmount(float amount)
//    {
//        targetAmount = Mathf.Clamp01(amount);
//    }

//    /// <summary>
//    /// Aplica inmediatamente (sin suavizado).
//    /// </summary>
//    public void ApplyAmountImmediate(float amount)
//    {
//        currentAmount = Mathf.Clamp01(amount);
//        targetAmount = currentAmount;
//        ApplyAmount(currentAmount);
//    }

//    private void ApplyAmount(float amount)
//    {
//        float h = Mathf.Lerp(0f, maxPanelHeight, amount);

//        if (usarSizeDelta)
//        {
//            Vector2 topSize = topPanel.sizeDelta;
//            topSize.y = h;
//            topPanel.sizeDelta = topSize;

//            Vector2 bottomSize = bottomPanel.sizeDelta;
//            bottomSize.y = h;
//            bottomPanel.sizeDelta = bottomSize;
//        }
//        else
//        {
//            // Mover anchoredPosition (ajusta según pivots)
//            Vector2 topPos = topPanel.anchoredPosition;
//            topPos.y = -h * 0.5f;
//            topPanel.anchoredPosition = topPos;

//            Vector2 bottomPos = bottomPanel.anchoredPosition;
//            bottomPos.y = h * 0.5f;
//            bottomPanel.anchoredPosition = bottomPos;
//        }
//    }
//}
