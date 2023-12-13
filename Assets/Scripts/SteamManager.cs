using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    public uint appId;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        try
        {
            Steamworks.SteamClient.Init(appId, true);
            Debug.Log("Steam init");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        try
        {
            Steamworks.SteamClient.Shutdown();
        }
        catch
        {

        }
    }
}
