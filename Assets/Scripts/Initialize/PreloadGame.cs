using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Unity.Netcode;
using Steamworks;
using LegionLords.API;
using TMPro;
using System.Threading.Tasks;
using System.Threading;
using static Steamworks.InventoryItem;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class PreloadGame : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _stateText;

    private async void Awake()
    {
        await Initialize();
    }

    private async Task Initialize()
    {

        await AwaitableUtil.WaitUntil(() => NetworkManager.Singleton && Instance.S && Server.S && Client.S && API.S);

        #if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            string argString = ClonesManager.GetArgument();

            if (argString.Contains("-server"))
            {
                RunServerStartup(MatchType.OneVSOne);
                return;
            }
            else if (argString.Contains("-host"))
            {
                await RunClientStartup(InstanceType.Server);
                return;
            }
        }
        #endif

        string[] commandLineArgs = Environment.GetCommandLineArgs();
        foreach (var arg in commandLineArgs)
        {
            if (arg == "-server")
            {
                RunServerStartup(MatchType.OneVSOne);
                return;
            }
        }
        
        await RunClientStartup(InstanceType.Client);
    }

    private void RunServerStartup(MatchType matchType)
    {
        Instance.S.Init(InstanceType.Server);
        Server.S.Init(matchType);
        SceneManager.LoadScene(Refs.SceneNames.Menu, LoadSceneMode.Single);
    }

    private async Task RunClientStartup(InstanceType instanceType)
    {

        Instance.S.Init(instanceType);

        if (instanceType == InstanceType.Server)
        {
            Server.S.Init(MatchType.OneVSOne);
        }

        SetStateText("loading...");

        SetStateText("validating steam connection...");

        if (!SteamClient.IsValid)
        {
            SetStateText("could not connect to steam... run steam before launching game");
            return;
        }

        SetStateText("validating account...");

        Client.S.Init();

        var payload = await API.S.GetAccount();
        if (payload.Result != UnityWebRequest.Result.Success)
        {
            SetStateText("could not connect to game server, try again later");
            return;
        }

        if (payload.Data == null)
        {
            Debug.Log("could not find account, creating new account from steam_id");
            payload = await API.S.CreateAccount();
        }

        if (payload.Result != UnityWebRequest.Result.Success || payload.Data == null)
        {
            Debug.LogError("ERROR: account was empty, this is an unexpected error");
            SetStateText("something went terribly wrong, try again later");
            return;
        }

        Client.S.Init(payload.Data.Id, SteamClient.SteamId, SteamClient.Name);
        Client.S.LegionManager.UpdateLegion();

        SceneManager.LoadScene(Refs.SceneNames.Menu, LoadSceneMode.Single);

        SetStateText("entering game...");

    }

    private void SetStateText(string text)
    {
        _stateText.text = text;
    }

    public void ClickQuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;        
        #endif

        MatchmakingManager.S.Disconnect();
        Application.Quit();
    }

}
