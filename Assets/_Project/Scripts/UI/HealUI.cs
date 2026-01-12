


using System;
using UnityEngine;
using UnityEngine.UI;

public class HealUI : TGTHMonoBehaviour
{
    [SerializeField] private Slider m_healSlider;
    [SerializeField] private Transform healUIPrefab;
    [SerializeField] private Transform parent;
    private HealthController m_controller;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        Transform healUI = Instantiate(healUIPrefab, parent);
        m_healSlider = healUI.GetComponent<Slider>();
        m_controller.OnHealthChanged += OnHealthChanged;

    }

    private void OnHealthChanged(float curheal, float maxheal)
    {
        float percent = maxheal > 0 ? curheal / maxheal : 0;
        m_healSlider.value = percent;
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        m_controller = GetComponent<HealthController>();
    }


}