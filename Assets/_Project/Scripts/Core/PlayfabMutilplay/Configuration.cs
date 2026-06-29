using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : Singleton<Configuration>
{
    public string PlayFabTitleId;
    public BuildType buildType;
    public string ipAddress = "";
    public ushort port = 0;
    public bool useEditor = false;
    public bool startwithHost = false;
    public bool isEditor;
    public bool playFabDebugging = false;
    public bool IsServerBuild() => buildType == BuildType.LOCAL_SERVER || buildType == BuildType.REMOTE_SERVER || isEditor;
    public bool IsClientBuild() => (buildType == BuildType.LOCAL_CLIENT || buildType == BuildType.REMOTE_CLIENT) && !isEditor;
    public bool IsClientLocalBuild() => buildType == BuildType.LOCAL_CLIENT;
    public bool IsClientRemoteBuild() => buildType == BuildType.REMOTE_CLIENT;
    public bool IsServerLocalBuild() => buildType == BuildType.LOCAL_SERVER;
    public bool IsServerRemoteBuild() => buildType == BuildType.REMOTE_SERVER;
    protected override void Awake()
    {
        base.Awake();
        if (useEditor)
        {
#if UNITY_EDITOR
            isEditor = true;
#else
            isEditor = false;
#endif
        }
        else
        {
            isEditor = false;
        }
    }
}

public enum BuildType
{
    LOCAL_CLIENT,
    REMOTE_CLIENT,
    LOCAL_SERVER,
    REMOTE_SERVER,
}