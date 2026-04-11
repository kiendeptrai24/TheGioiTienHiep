using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIItemConditionLevelup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image checkImg;
    [SerializeField] private Image unCheckImg;

    public void Setup(string desc, bool isSatisfied)
    {
        if (description != null)
            description.text = desc;

        SetState(isSatisfied);
    }

    public void SetSatisfied()
    {
        if (checkImg != null)
            checkImg.gameObject.SetActive(true);

        if (unCheckImg != null)
            unCheckImg.gameObject.SetActive(false);
    }

    public void SetUnsatisfied()
    {
        if (checkImg != null)
            checkImg.gameObject.SetActive(false);

        if (unCheckImg != null)
            unCheckImg.gameObject.SetActive(true);
    }

    public void SetState(bool isSatisfied)
    {
        if (isSatisfied)
            SetSatisfied();
        else
            SetUnsatisfied();
    }
}