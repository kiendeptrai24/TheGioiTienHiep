using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MonsterOptionUI : MonoBehaviour
{
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private GameObject root;
    private PlayerChoseObject choseObject;
    private void Awake()
    {
        choseObject = PlayerChoseObject.Instance;
        choseObject.OnEntityClicked += OnEntityClicked;
        leaveButton.onClick.AddListener(() =>
        {
            OnLeaveClicked();
        });
        attackButton.onClick.AddListener(() =>
        {
            OnAttackClicked();
        });
    }

    private void OnEntityClicked(EntityClickable entity)
    {
        Show();
    }

    public void OnLeaveClicked()
    {
        Hide();
    }

    public void OnAttackClicked()
    {
        choseObject.RequestBattleSimulator();
        Hide();
    }
    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
