using UnityEngine;

public class InitGameScene : TGTHMonoBehaviour
{
    [SerializeField] private string sceneName;
    protected override void Start()
    {
        base.Start();
        SceneLoadManager.Instance.LoadScene(sceneName);
    }
}
