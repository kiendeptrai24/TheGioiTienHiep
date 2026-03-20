using System;
using System.Collections.Generic;
using DuloGames.UI;
using PlayFab;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class ForgotPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button registerBtn;
        [SerializeField] private Button navLoginBtn;
        [SerializeField] private TextMeshProUGUI descriptionErrorTxt;
        [SerializeField] private TMP_InputField emailField;
        public event Action<ForgotPasswordData> OnStartClicked;
        protected override void Awake()
        {
            base.Awake();
            navLoginBtn.onClick.AddListener(() =>
            {
                ForgotPasswordData data = new ForgotPasswordData();
                data.email = emailField.text;
                data.titleId = PlayFabSettings.staticSettings.TitleId;
                OnStartClicked?.Invoke(data);
            });
        }
        public void ShowError(string error)
        {
            descriptionErrorTxt.text = "+ " + error;
        }

    }
}