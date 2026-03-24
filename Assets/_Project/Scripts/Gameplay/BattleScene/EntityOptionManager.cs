using System;
using FeatureToggles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EntityOptionManager : MonoBehaviour
{
    [SerializeField] private GameObject root;
    private PlayerChoseObject choseObject;
    [SerializeField] private FeatureId gate = FeatureId.WorldClick_Enabled;
    private FeatureManager _mgr;
    [SerializeField] private MineOptionUI mineOpptionUI;
    [SerializeField] private MonsterOptionUI monsterOptionUI;
    private void Awake()
    {
        choseObject = PlayerChoseObject.Instance;
        choseObject.OnEntityClicked += OnEntityClicked;
    }
    private void Start()
    {
        _mgr = FeatureManager.Instance;
        LockUI(_mgr.IsEnabled(gate));
        _mgr.OnFeatureEffectiveChanged += OnChanged;
    }

    private void OnChanged(FeatureId id, bool arg2)
    {
        if (id.Equals(gate)) LockUI(arg2);
    }
    private void LockUI(bool unlock)
    {
        if (unlock)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void OnEntityClicked(EntityClickable entity)
    {
        Show();
        if (entity.entityWorldType == EntityWorldType.Mine)
        {
            mineOpptionUI.SetEntity(choseObject);
            monsterOptionUI.Hide();
        }
        else
        {
            monsterOptionUI.SetEntity(choseObject);
            mineOpptionUI.Hide();
        }
    }
    public void OnInfoClicked()
    {

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
        if (!_mgr.IsEnabled(gate)) return;
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
