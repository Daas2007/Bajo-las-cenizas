using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUIPanel : MonoBehaviour, IUIPanel
{
    public string PanelName { get => _panelName; set => _panelName = value; }

    [Header("Panel Info")]
    [SerializeField] string _panelName;
    [SerializeField] bool resetPanel = false;

    //[Header("Traduction")]
    //public List<LanguageConfiguration> textsConfiguration;

    [Header("Animations")]
    [SerializeField] Animator animator;

    int hideHash = Animator.StringToHash("Hide");
    int showHash = Animator.StringToHash("Show");
    int idleHash = Animator.StringToHash("Idle");

    public virtual void Start()
    {
    }
    public virtual void Awake()
    {
        TryGetComponent(out animator);
    }
    public virtual void Show()
    {
        if (resetPanel)
        {
            OnReset();
        }
    }
    public virtual void OnReset()
    {
    }
    public virtual void Hide()
    {
    }
    public IEnumerator HideCoroutine(bool instant)
    {
        if (animator == null)
            yield break;

        bool animation = animator.HasState(0, hideHash);
        if (animation && !instant)
        {
            animator?.Play(hideHash);
            yield return WaitForAnimation("Hide");
        }
        else
            yield break;
    }
    public IEnumerator ShowCoroutine(bool instant)
    {
        if (animator == null)
            yield break;

        bool animation = animator.HasState(0, instant ? idleHash : showHash);
        if (animation)
        {
            animator?.Play(instant ? idleHash : showHash);
            if (instant)
                yield break;
            yield return WaitForAnimation("Show");
        }
        else
            yield break;
    }
    protected IEnumerator WaitForAnimation(string stateName)
    {
        yield return null;
        var clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (clipInfo.IsName(stateName) && clipInfo.normalizedTime < 1f)
        {
            yield return null;
            clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
    //public void UpdateText(LanguagesEnum language)
    //{
    //    foreach (var item in textsConfiguration)
    //    {
    //        if (item.configuration.GetText(language, out string newText))
    //        {
    //            item.text.text = newText;
    //        }
    //    }
    //}
    //private void OnEnable()
    //{
    //    ApplicationEvents.LanguageChanged += UpdateText;
    //}
    //private void OnDisable()
    //{
    //    ApplicationEvents.LanguageChanged -= UpdateText;
    //}
}
