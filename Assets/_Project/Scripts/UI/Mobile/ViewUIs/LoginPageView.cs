using System;
using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class LoginPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button navToRegisterBtn;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TextMeshProUGUI descriptionErrorTxt;
        public event Action<LoginData> OnLoginClicked;

        protected override void Awake()
        {
            base.Awake();
            loginBtn.onClick.AddListener(() =>
            {
                LoginData data = new LoginData();
                data.email = emailField.text;
                data.password = passwordField.text;
                OnLoginClicked?.Invoke(data);
            });
        }
        public void ShowMessege(string error)
        {
            descriptionErrorTxt.text = "+ " + error;
        }

    }
}