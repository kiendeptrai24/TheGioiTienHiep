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
        [SerializeField] private Button startBtn;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TextMeshProUGUI descriptionTxt;
        public event Action OnStartClicked;
        public event Action<string> OnFieldEndEdit;
        public event Action<string> OnPasswordFieldEndEdit;

        protected override void Awake()
        {
            base.Awake();
            startBtn.onClick.AddListener(() => OnStartClicked?.Invoke());
            nameField.onEndEdit.AddListener((value) => OnFieldEndEdit?.Invoke(value));
            passwordField.onEndEdit.AddListener((value) => OnPasswordFieldEndEdit?.Invoke(value));
        }

    }
}