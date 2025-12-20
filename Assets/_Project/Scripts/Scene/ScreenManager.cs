using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public struct NavigationData
{
    public string ScreenName;
    public object Data;
}

public abstract class ScreenManager : TGTHMonoBehaviour
{
    protected Stack<NavigationData> m_NavigationStack = new();
    protected Dictionary<string, GameObject> m_Screens = new();
    protected GameObject m_CurrentScreen;
    [SerializeField] protected string defaultScreen;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
    }
    private void OnEnable() {
        StartUI(defaultScreen);
    }
    protected void StartUI(string defaultScreen)
    {
        foreach (var ui in m_Screens)
            Hide(ui.Value);
        if (m_Screens.Count == 0) return;
        NavigateTo(defaultScreen);
    }


    public void NavigateTo(string screenName, object data = null)
    {
        m_NavigationStack.Push(new NavigationData { ScreenName = screenName, Data = data });
        OnStackChanged();
    }

    public void NavigateBack()
    {
        if (m_NavigationStack.Count > 0)
        {
            m_NavigationStack.Pop();
            OnStackChanged();
        }
    }
    [ContextMenu("Hide UI")]
    public void HideUI()
    {
        while (m_NavigationStack.Count > 1)
        {
            NavigateBack();
        }
        if (m_NavigationStack.Count == 1 && m_NavigationStack.Peek().ScreenName == "GameMenu")
        {
            var screenName = m_NavigationStack.Peek().ScreenName;
            if (m_Screens.ContainsKey(screenName))
            {
                m_CurrentScreen = m_Screens[screenName];
                m_CurrentScreen.SetActive(false);
                m_CurrentScreen = null;
            }
            m_NavigationStack.Pop();
        }

    }
    public void OnStackChanged()
    {
        if (m_NavigationStack.Count > 0)
        {
            var screenName = m_NavigationStack.Peek().ScreenName;

            if (m_Screens.ContainsKey(screenName))
            {
                if (m_CurrentScreen != null)
                {
                    m_CurrentScreen.SetActive(false);
                }

                m_CurrentScreen = m_Screens[screenName];
                m_CurrentScreen.SetActive(true);
            }
        }
    }
    public object GetNavigationData()
    {
        if (m_NavigationStack.Count > 0)
        {
            return m_NavigationStack.Peek().Data;
        }

        return null;
    }
    public int GetStackSize() => m_NavigationStack.Count;
    public string GetCurrentScreen() => m_CurrentScreen.gameObject.name;
    private void Show(GameObject gameObject) => gameObject.SetActive(true);
    private void Hide(GameObject gameObject) => gameObject.SetActive(false);
}