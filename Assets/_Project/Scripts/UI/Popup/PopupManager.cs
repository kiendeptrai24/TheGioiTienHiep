using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : Singleton<PopupManager>
{
    private Dictionary<Type, IPopup> _popups = new Dictionary<Type, IPopup>();
    private List<IPopup> _popupStack = new List<IPopup>();

    protected override void Awake()
    {
        base.Start();
        RegisterAllPopups();
        DontDestroyOnLoad(gameObject);
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
            Type popupType = popup.GetType();
            _popups[popupType] = popup;
        }
    }

    public T GetPopup<T>() where T : class, IPopup
    {
        if (_popups.TryGetValue(typeof(T), out IPopup popup))
        {
            return popup as T;
        }
        return null;
    }

    public void ShowPopup<T>(IPopup popup) where T : class, IPopup
    {
        if (popup != null)
        {
            popup.Show();
            _popupStack.Add(popup);

            if (popup is MonoBehaviour mb)
            {
                mb.transform.SetAsLastSibling();
            }
        }
    }

    public void HidePopup(IPopup popup)
    {
        if (popup != null)
        {
            popup.Hide();
            _popupStack.Remove(popup);
        }
    }

    public void HideAllPopups()
    {
        foreach (var popup in _popupStack.ToList())
        {
            popup.Hide();
        }
        _popupStack.Clear();
    }
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, Scene arg1)
    {
        RegisterAllPopups();
    }
}