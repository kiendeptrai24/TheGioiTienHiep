using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class ProfilePageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button copyUserIdBtn;
        [SerializeField] private TextMeshProUGUI userIdTxt;
        [SerializeField] private TMP_InputField userIdField;
        [SerializeField] private TMP_InputField userNameField;
        public event Action<string> OnUserIdChanged;
        public event Action<string> OnUserNameChanged;
        protected override void Awake()
        {
            base.Awake();
            // userIdField.onValueChanged.AddListener((string text) => OnUserIdChanged?.Invoke(text));
            userNameField.onEndEdit.AddListener((string text) => OnUserNameChanged?.Invoke(text));
            copyUserIdBtn.onClick.AddListener(CopyUserId);
        }
        public void ShowUserId(string userId)
        {
            userIdField.text = userId;
        }
        public void ShowUserName(string userName)
        {
            userNameField.text = userName;
            userIdTxt.text = userName;
        }
        public void ShowUser(ProfileUser user)
        {
            ShowUserId(user.userId);
            ShowUserName(user.userName);
        }
        private void CopyUserId()
        {
            if (string.IsNullOrEmpty(userIdField.text))
                return;

            GUIUtility.systemCopyBuffer = userIdField.text;
        }
    }
}
