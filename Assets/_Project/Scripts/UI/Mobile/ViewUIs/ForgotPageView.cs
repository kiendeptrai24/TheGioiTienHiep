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
        [SerializeField] private Button backBtn;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private TextMeshProUGUI descriptionErrorTxt;
        [SerializeField] private TMP_InputField emailField;
        public string descriptionTurtorial = "Vui Lòng nhập Email của tài khoản đã quên mật khẩu";
        public event Action<ForgotPasswordData> OnStartClicked;
        protected override void Awake()
        {
            base.Awake();
            confirmBtn.onClick.AddListener(() =>
            {
                ForgotPasswordData data = new ForgotPasswordData();
                data.email = emailField.text;
                data.titleId = PlayFabSettings.staticSettings.TitleId;
                OnStartClicked?.Invoke(data);
            });
        }
        public void ShowMessege(string error)
        {
            descriptionErrorTxt.text = "+ " + error;
        }
        public void HideMessege()
        {
            descriptionErrorTxt.text = descriptionTurtorial;
        }

    }
}