using FeatureToggles;
using UnityEngine;

public class FeatureInputHandlerBinder : TGTHMonoBehaviour
{
    private FeatureManager _mgr;
    [SerializeField] private InputManager _inputManager;

    protected override void Start()
    {
        base.Start();
        if (_inputManager == null)
            _inputManager = GetComponent<InputManager>();
        _mgr = FeatureManager.Instance;
        _mgr.OnFeatureEffectiveChanged += OnFeatureChanged;

        // Apply trạng thái hiện tại ngay khi start (quan trọng)
        ApplyAll();
    }
    private void OnDestroy()
    {
        if (_mgr != null)
            _mgr.OnFeatureEffectiveChanged -= OnFeatureChanged;
    }

    private void OnFeatureChanged(FeatureId id, bool enabled)
    {
        switch (id)
        {
            case FeatureId.WorldClick_Enabled:
                ApplyPlayer(enabled);
                break;

            case FeatureId.BattleScene_Enabled:
                ApplyBattle(enabled);
                break;
        }
    }

    private void ApplyAll()
    {
        // Nếu bạn có nhiều feature ảnh hưởng input, apply hết:
        ApplyPlayer(_mgr.IsEnabled(FeatureId.WorldClick_Enabled));
        ApplyBattle(_mgr.IsEnabled(FeatureId.BattleScene_Enabled));
    }

    private void ApplyPlayer(bool enabled)
    {
        Debug.Log("player input " + (enabled ? "unlocked" : "locked"));

        if (enabled) _inputManager.TurnOnPlayerInput();
        else _inputManager.TurnOffPlayerInput();

        Debug.Log("Player map enabled: " + _inputManager.inputHandler.Player.enabled);
    }

    private void ApplyBattle(bool enabled)
    {
        Debug.Log("battle input " + (enabled ? "unlocked" : "locked"));

        // Nếu battle feature cũng chỉ điều khiển Player map thì ok,
        // nhưng thường battle nên là map khác (Battle), bạn cân nhắc tách map.
        if (enabled) _inputManager.TurnOnPlayerInput();
        else _inputManager.TurnOffPlayerInput();
    }
}