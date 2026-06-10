using TMPro;
using UnityEngine;

public class MinimapPossitionUI : TGTHMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posTxt;
    private PlayerPositionTracker localPlayerPositionTracker;
    protected override void Start()
    {
        localPlayerPositionTracker = PlayerPositionTracker.Instance;
        localPlayerPositionTracker.OnPositionChanged += OnPlayerPosChanged;
    }
    private void OnDestroy()
    {
        if (localPlayerPositionTracker == null) return;
        localPlayerPositionTracker.OnPositionChanged -= OnPlayerPosChanged;
    }
    private void OnPlayerPosChanged(int xPos, int yPos)
    {
        posTxt.text = xPos.ToString() + ":" + yPos.ToString();
    }
}