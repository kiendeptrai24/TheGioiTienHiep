using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public event Action<float> process;
    protected override void Awake()
    {
        DontDestroyOnLoad(this);
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
                Debug.Log("Loading scene unloaded");
            };
        }
    }

    private IEnumerator ProcessRegularSceneLoading(string sceneToLoad, bool useLoadScene = true, bool WaitForSeconds = true)
    {
        if (useLoadScene)
        {
            SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
            if (WaitForSeconds)
                yield return new WaitForSeconds(1f);
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
            yield return null;


        while (!loadOp.isDone)
        {
            process.Invoke(loadOp.progress);
            yield return null;
            loadOp.allowSceneActivation = true;
        }

        if (useLoadScene)
        {
            UnLoadScene("LoadingScene");
        }
    }
    #endregion

}