using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public event Action<float> process;
    private Scene sceneMain;
    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        NetworkManager.Singleton.NetworkConfig.EnableSceneManagement = false;
    }
    #region Network Scene

    public void SubscribeOnNetworkEvents()
    {
        //On host prepared scene to load
        NetworkManager.Singleton.SceneManager.OnSynchronize += (clientId) =>
        {
            //Works on client side only
            if (NetworkManager.Singleton.LocalClientId == clientId)
                SceneManager.LoadScene("LoadingScene");

        };

        //On host loading scene
        NetworkManager.Singleton.SceneManager.OnLoad += (clientId, sceneName, mode, sceneLoadOperation) =>
        {
            StartCoroutine(ProcessNetworkSceneLoading(sceneLoadOperation));
        };
    }

    public void LoadNetworkScene(string sceneName)
    {
        //Switch to loading scene first
        SceneManager.LoadScene("LoadingScene");

        SubscribeOnNetworkEvents();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    private IEnumerator ProcessNetworkSceneLoading(AsyncOperation asyncOperation)
    {
        yield return asyncOperation;
        SceneManager.UnloadSceneAsync("LoadingScene");
    }
    #endregion

    #region Scene

    public void LoadRegularScene(string sceneName, bool useLoadScreen = true)
    {
        StartCoroutine(ProcessRegularSceneLoading(sceneName, useLoadScreen));
    }

    public void UnLoadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene == null) return;
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene).completed += _ =>
            {
                if (!sceneMain.IsValid() || "LoadingScene" == sceneName) return;
                foreach (var go in sceneMain.GetRootGameObjects())
                    go.SetActive(true);
                Debug.Log("Loading scene unloaded");
            };
        }
    }

    private IEnumerator ProcessRegularSceneLoading(string sceneToLoad, bool useLoadScene = true, bool waitForSeconds = true)
    {
        Scene oldScene = SceneManager.GetActiveScene();

        if (useLoadScene)
        {
            SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
            if (waitForSeconds) yield return new WaitForSeconds(1f);
        }

        foreach (var go in oldScene.GetRootGameObjects())
            go.SetActive(false);

        var loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            process?.Invoke(loadOp.progress);
            yield return null;
        }

        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
        {
            process?.Invoke(loadOp.progress);
            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
        if (newScene.IsValid())
            SceneManager.SetActiveScene(newScene);

        if (useLoadScene)
            UnLoadScene("LoadingScene");

        sceneMain = oldScene;
    }
    #endregion

}