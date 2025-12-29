using UnityEngine;

public class ScreenCopy : TGTHMonoBehaviour
{
    public GameObject pageRoot;
    public BackButton backRootBtn;
    public ScreenManager SceneManagerRoot;
    public ScreenManager SceneManagerCopy;

    protected override void Awake()
    {
        base.Awake();

    }
    private void OnEnable() {
        pageRoot.SetActive(true);
        backRootBtn.screenManager = SceneManagerCopy;
    }
    private void OnDisable() {
        pageRoot.SetActive(false);
        backRootBtn.screenManager = SceneManagerRoot;
    }
}
