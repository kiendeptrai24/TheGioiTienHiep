


using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViralUI : TGTHMonoBehaviour
{
    [SerializeField] private Slider m_viralHealthSlider;
    [SerializeField] private Slider m_viralManaSlider;
    [SerializeField] private Slider m_viralSpiritSlider;
    [SerializeField] private TextMeshProUGUI m_viralHealthText;
    [SerializeField] private TextMeshProUGUI m_viralManaText;
    [SerializeField] private TextMeshProUGUI m_viralSpiritText;
    public void OnVitalChanged(VitalType type, int maxValue, int curValue)
    {
        switch (type)
        {
            case VitalType.Health:
                m_viralHealthSlider.value = (float)curValue / maxValue;
                m_viralHealthText.text = curValue.ToString();
                break;
            case VitalType.Mana:
                m_viralManaSlider.value = (float)curValue / maxValue;
                m_viralManaText.text = curValue.ToString();
                break;
            case VitalType.Spirit:
                m_viralSpiritSlider.value = (float)curValue / maxValue;
                m_viralSpiritText.text = curValue.ToString();
                break;
        }
    }

}