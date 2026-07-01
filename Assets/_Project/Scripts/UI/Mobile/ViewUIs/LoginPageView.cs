using System;
using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class LoginPageView : TGTHMonoBehaviour
    {
        private const string MessagePrefix = "+ ";
        private const string LoadingButtonText = "Đang tải";
        private const float LoadingAnimationIntervalSeconds = 0.35f;

        [SerializeField] private Button loginBtn;
        [SerializeField] private Button navToRegisterBtn;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TextMeshProUGUI descriptionErrorTxt;
        [SerializeField] private TextMeshProUGUI loginButtonText;
        public event Action<LoginData> OnLoginClicked;

        private Coroutine loadingButtonCoroutine;
        private string defaultLoginButtonText;

        protected override void Awake()
        {
            base.Awake();
            if (loginButtonText == null && loginBtn != null)
            {
                loginButtonText = loginBtn.GetComponentInChildren<TextMeshProUGUI>();
            }

            if (loginButtonText != null)
            {
                defaultLoginButtonText = loginButtonText.text;
            }

            loginBtn.onClick.AddListener(NotifyLoginClicked);
        }

        private void OnDestroy()
        {
            StopLoadingButtonAnimation();
            loginBtn.onClick.RemoveListener(NotifyLoginClicked);
        }

        private void NotifyLoginClicked()
        {
            if (loginBtn != null && !loginBtn.interactable)
            {
                return;
            }

            var data = new LoginData
            {
                email = emailField != null ? emailField.text.Trim() : string.Empty,
                password = passwordField != null ? passwordField.text : string.Empty
            };

            OnLoginClicked?.Invoke(data);
        }

        public void HideMessege()
        {
            descriptionErrorTxt.text = "";
        }

        public void ShowMessege(string error)
        {
            descriptionErrorTxt.text = string.IsNullOrEmpty(error) ? "" : MessagePrefix + error;
        }

        public void ShowAccount(string email, string password)
        {
            emailField.text = email == null ? "" : email;
            passwordField.text = password == null ? "" : password;
        }

        public void SetLoginInteractable(bool interactable)
        {
            if (loginBtn != null)
            {
                loginBtn.interactable = interactable;
            }
        }

        public void SetInputsInteractable(bool interactable)
        {
            if (emailField != null)
            {
                emailField.interactable = interactable;
            }

            if (passwordField != null)
            {
                passwordField.interactable = interactable;
            }

            if (navToRegisterBtn != null)
            {
                navToRegisterBtn.interactable = interactable;
            }
        }

        public void SetLoginInProgress(bool inProgress)
        {
            SetLoginInteractable(!inProgress);
            SetInputsInteractable(!inProgress);
            SetLoadingButtonState(inProgress);
        }

        private void SetLoadingButtonState(bool inProgress)
        {
            if (loginButtonText == null)
            {
                return;
            }

            if (inProgress)
            {
                if (loadingButtonCoroutine == null)
                {
                    loadingButtonCoroutine = StartCoroutine(AnimateLoadingButtonText());
                }
                return;
            }

            StopLoadingButtonAnimation();
            loginButtonText.text = defaultLoginButtonText;
        }

        private IEnumerator AnimateLoadingButtonText()
        {
            var wait = new WaitForSeconds(LoadingAnimationIntervalSeconds);
            var dotCount = 1;

            while (true)
            {
                loginButtonText.text = LoadingButtonText + new string('.', dotCount);
                dotCount = dotCount % 3 + 1;
                yield return wait;
            }
        }

        private void StopLoadingButtonAnimation()
        {
            if (loadingButtonCoroutine == null)
            {
                return;
            }

            StopCoroutine(loadingButtonCoroutine);
            loadingButtonCoroutine = null;
        }

    }
}
