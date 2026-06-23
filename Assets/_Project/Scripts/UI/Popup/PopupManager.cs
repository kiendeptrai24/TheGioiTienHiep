using System;
using System.Collections.Generic;
using System.Linq;
using FeatureToggles;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : Singleton<PopupManager>
{
    private readonly Dictionary<Type, IPopup> _popups = new();
    private readonly List<IPopup> _popupStack = new();

    // private FeatureManager _mgr;
    // private const string BLOCK_SRC = "Popup"; // lý do chặn

    protected override void Awake()
    {
        base.Awake(); // (bạn đang gọi base.Start() là sai vòng đời, nên để Awake)
        DontDestroyOnLoad(gameObject);

        // _mgr = FeatureManager.Instance;   // lấy instance ở đây an toàn hơn
        RegisterAllPopups();
    }

    private void RegisterAllPopups()
    {
        var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        var allPopups = allMonoBehaviours
            .Where(m => m is IPopup)
            .Cast<IPopup>()
            .ToList();

        foreach (var popup in allPopups)
        {
            _popups[popup.GetType()] = popup;
        }
    }

    public T GetPopup<T>() where T : class, IPopup
    {
        return _popups.TryGetValue(typeof(T), out var popup) ? popup as T : null;
    }

    public void ShowPopup<T>(IPopup popup) where T : class, IPopup
    {
        if (popup == null)
        {
            Debug.Log("popup is null");
            return;
        }
        // Nếu popup đã ở trong stack rồi thì khỏi add lại (tránh double)
        if (_popupStack.Contains(popup)) return;
        HideAllPopups();
        popup.Show();
        _popupStack.Add(popup);

        if (popup is MonoBehaviour mb)
            mb.transform.SetAsLastSibling();
    }

    public void HidePopup(IPopup popup)
    {
        if (popup == null) return;
        if (!_popupStack.Remove(popup)) return;

        popup.Hide();
    }

    public void HideAllPopups()
    {
        foreach (var popup in _popupStack.ToList())
            popup.Hide();

        _popupStack.Clear();
    }

    private void OnEnable() => SceneManager.activeSceneChanged += OnSceneLoaded;
    private void OnDisable() => SceneManager.activeSceneChanged -= OnSceneLoaded;

    private void OnSceneLoaded(Scene oldScene, Scene newScene)
    {
        RegisterAllPopups();
    }
}