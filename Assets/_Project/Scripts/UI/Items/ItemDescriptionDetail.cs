using TMPro;
using UnityEngine;


public class ItemDescriptionDetail : TGTHMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    public void SetDescription(string description)
    {
        descriptionTxt.text = description;
    }
}