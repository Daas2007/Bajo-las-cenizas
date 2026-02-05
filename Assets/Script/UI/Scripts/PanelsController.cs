using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsController : BaseUIPanel
{
    public List<BaseUIPanel> Panels { get => _panels; set => _panels = value; }
    [Header("Content")]
    [SerializeField] List<BaseUIPanel> _panels;
    [Header("Info")]
    [SerializeField] BaseUIPanel currentPanel;
    BaseUIPanel lastPanel;
    [SerializeField] BaseUIPanel initialPanel;
    [SerializeField] bool transition = false;

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        ShowInitialPanel();
    }
    public override void OnReset()
    {
        ShowInitialPanel();
    }
    public void ShowInitialPanel()
    {
        if (initialPanel != null)
        {
            ChangePanel(initialPanel.PanelName, null, true);
        }
        else
        {
            CloseCurrentPanel(true);
        }
    }

    public void ChangePanel(string panelName, Action finishAction = null, bool instant = false, bool simultaneous = true)
    {
        StartCoroutine(ChangePanelSimultaneousCoroutine(panelName, finishAction, instant, simultaneous));
    }
    public void ChangePanel(string panelName, Action finishAction = null)
    {
        StartCoroutine(ChangePanelSimultaneousCoroutine(panelName, finishAction));
    }
    public void ChangePanel(string panelName)
    {
        StartCoroutine(ChangePanelSimultaneousCoroutine(panelName));
    }
    public IEnumerator ChangePanelSimultaneousCoroutine(
     string panelName,
     Action finishAction = null,
     bool instant = false,
     bool simultaneous = true)
    {
        if (transition)
            yield break;

        BaseUIPanel nextPanel = Panels.Find(p => p.PanelName == panelName);
        if (nextPanel == null || nextPanel == currentPanel)
            yield break;

        transition = true;

        BaseUIPanel previousPanel = currentPanel;
        lastPanel = previousPanel;

        Coroutine hideCoroutine = null;
        Coroutine showCoroutine = null;

        // Hide anterior
        if (previousPanel != null)
            hideCoroutine = StartCoroutine(previousPanel.HideCoroutine(instant));

        // Show nuevo (si es simultáneo)
        if (simultaneous)
            showCoroutine = StartShow(nextPanel, instant);

        // Esperamos Hide
        if (hideCoroutine != null)
            yield return hideCoroutine;

        // Desactivamos anterior
        if (previousPanel != null)
            previousPanel.gameObject.SetActive(false);

        // Show nuevo (si NO es simultáneo)
        if (!simultaneous)
            showCoroutine = StartShow(nextPanel, instant);

        // Esperamos Show
        if (showCoroutine != null)
            yield return showCoroutine;

        finishAction?.Invoke();
        transition = false;
    }
    private Coroutine StartShow(BaseUIPanel panel, bool instant)
    {
        currentPanel = panel;
        panel.gameObject.SetActive(true);
        panel.Show();
        return StartCoroutine(panel.ShowCoroutine(instant));
    }
    public void CloseCurrentPanel(bool instant)
    {
        StartCoroutine(CloseCurrentPanelCoroutine(instant));
    }
    public void CloseCurrentPanel()
    {
        StartCoroutine(CloseCurrentPanelCoroutine(false));
    }
    public IEnumerator CloseCurrentPanelCoroutine(bool instant)
    {
        yield return currentPanel?.HideCoroutine(instant);
        currentPanel?.gameObject.SetActive(false);
        lastPanel = currentPanel;
        currentPanel = null;
    }
}
