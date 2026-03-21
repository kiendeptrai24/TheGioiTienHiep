using System;
using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class RegisterPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button registerBtn;
        [SerializeField] private Button navLoginBtn;
        [SerializeField] private TextMeshProUGUI descriptionErrorTxt;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField confirmPasswordField;
        public event Action<RegisterData> OnRegisterClicked;


        protected override void Awake()
        {
            base.Awake();
            registerBtn.onClick.AddListener(() =>
            {
                RegisterData data = new RegisterData();
                data.email = emailField.text;
                data.password = passwordField.text;
                data.confirmPassword = confirmPasswordField.text;
                OnRegisterClicked?.Invoke(data);
            });
        }
        public void HideMessege()
        {
            descriptionErrorTxt.text = "";
        }
        public void ShowMessege(string error)
        {
            descriptionErrorTxt.text = "+ " + error;
        }

    }
}