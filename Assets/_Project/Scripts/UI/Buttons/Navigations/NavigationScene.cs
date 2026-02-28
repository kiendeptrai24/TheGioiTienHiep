using UnityEngine;

public class NavigationScene : ActionButton
{
    
    [SerializeField] private string m_SceneName;
    override public void OnClick()
    {
        SceneLoadManager.Instance.LoadRegularScene(m_SceneName,false);
    }
}