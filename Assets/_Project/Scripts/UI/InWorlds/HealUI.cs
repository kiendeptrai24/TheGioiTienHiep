


using System;
using UnityEngine;
using UnityEngine.UI;

public class HealUI : TGTHMonoBehaviour
{
    [SerializeField] private ViralUI viralPrefabUI;
    private ViralUI viralUI;
    [SerializeField] private Transform parent;
    private Champion_Heal championViral;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        viralUI = Instantiate(viralPrefabUI, parent);
        championViral.OnHealthChanged += OnHealthChanged;
        championViral.OnManaChanged += OnManaChanged;
        championViral.OnSpiritChanged += OnSpiritChanged;
    }

    private void OnManaChanged(float maxheal, float curheal)
    {
        viralUI.OnVitalChanged(VitalType.Mana, (int)maxheal, (int)curheal);
    }

    private void OnSpiritChanged(float maxheal, float curheal)
    {
        viralUI.OnVitalChanged(VitalType.Spirit, (int)maxheal, (int)curheal);
    }

    public void OnHealthChanged(float maxheal, float curheal)
    {
        viralUI.OnVitalChanged(VitalType.Health, (int)maxheal, (int)curheal);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        championViral = GetComponent<Champion_Heal>();
    }


}