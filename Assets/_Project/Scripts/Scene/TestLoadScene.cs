using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadScene : Singleton<TestLoadScene>
{
    public string sceneName;
    private SceneLoadManager sceneLoadManager;
    public AsyncOperation process;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        sceneLoadManager = SceneLoadManager.Instance;
        sceneLoadManager.process += UpdateProcess;
    }

    private void UpdateProcess(float value)
    {
        Debug.Log(value);
    }

    protected override void Start()
    {
        base.Start();
    }

    [ContextMenu("Load Scene")]
    public void LoadScene()
    {
        sceneLoadManager.LoadRegularScene(sceneName);
    }

    [ContextMenu("Reload Scene")]
    public void UnLoadScene()
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene == null) return;
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene).completed += _ =>
            {
                Debug.Log("Scene unloaded");
            };
        }
    }
}
