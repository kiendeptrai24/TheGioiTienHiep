
using System;
using System.Collections.Generic;
using UnityEngine;
namespace TGTH.Mobile
{
    public class LoginPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private LoginPageView view;
        private InventoryCenterManager inventoryCenterManager;
        protected override void Awake()
        {
            base.Awake();
            inventoryCenterManager = InventoryCenterManager.Instance;
            view.OnStartClicked += OnStartClicked;
            view.OnFieldEndEdit += OnFieldEndEdit;
            view.OnPasswordFieldEndEdit += OnPasswordFieldEndEdit;
        }

        private void OnPasswordFieldEndEdit(string obj)
        {
        }

        private void OnFieldEndEdit(string obj)
        {
        }

        private void OnStartClicked()
        {
            
        }
    }
}