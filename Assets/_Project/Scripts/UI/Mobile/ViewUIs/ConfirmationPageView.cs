using System;
using System.Collections.Generic;
using DuloGames.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class ConfirmationPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button okBtn;
        [SerializeField] private Button exitBtn;
        public event Action OnOkClicked;
        public event Action OnExitClicked;
        protected override void Awake()
        {
            base.Awake();
            okBtn.onClick.AddListener(() => OnOkClicked?.Invoke());
            exitBtn.onClick.AddListener(() => OnExitClicked?.Invoke());
        }

    }
}