using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Rendering;
using Netcode.Transports.Facepunch;
using Steamworks;

public class DedicatedServer : Server
{

    protected override void Awake()
    {
        if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) return;

        Init(MatchType.OneVSOne);
        base.Awake();
    }

    public override void Init(MatchType matchType)
    {
        if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) return;

        base.Init(matchType);

        // Make sure the SteamServer is Shutdown before attempting to start a new server.
        SteamServer.Shutdown();

        SteamServer.OnSteamServersConnected      += SteamServer_OnSteamServersConnected;
        SteamServer.OnSteamServersDisconnected   += SteamServer_OnSteamServersDisconnected;
        SteamServer.OnSteamServerConnectFailure  += SteamServer_OnSteamServerConnectFailure;
        SteamServer.OnValidateAuthTicketResponse += SteamServer_OnValidateAuthTicketResponse;

        Debug.Log("@---| INITIALIZING SERVER |---@");

        NetworkManager.Singleton.GetComponent<UnityTransport>().enabled = false;
        NetworkManager.Singleton.GetComponent<FacepunchTransport>().enabled = true;

        NetworkManager.Singleton.NetworkConfig.NetworkTransport = FindAnyObjectByType<FacepunchTransport>();

        SteamServerInit steamServerInit = new SteamServerInit
        {
            DedicatedServer = true,
            ModDir = Refs.GameTitle,
            GameDescription = "Auto-RTS, Tug-of-war",
            GamePort = 27015,
            QueryPort = 27016,
            Secure = true,
            VersionString = "1.0.0.0",
            IpAddress = null,
            SteamPort = 0,
        };

        SteamServer.Init(Refs.SteamAppID, steamServerInit);

        SteamServer.ServerName = Refs.GameTitle;
        SteamServer.MaxPlayers = 6;

        SteamServer.LogOnAnonymous();

        if (!NetworkManager.Singleton.StartServer())
        {
            Debug.LogError("NetworkManager failed to start server");
        }
    }

    protected override void OnDisable()
    {
        if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) return;

        base.OnDisable();

        SteamServer.OnSteamServersConnected      -= SteamServer_OnSteamServersConnected;
        SteamServer.OnSteamServersDisconnected   -= SteamServer_OnSteamServersDisconnected;
        SteamServer.OnSteamServerConnectFailure  -= SteamServer_OnSteamServerConnectFailure;
        SteamServer.OnValidateAuthTicketResponse -= SteamServer_OnValidateAuthTicketResponse;

        if (SteamServer.IsValid) SteamServer.Shutdown();
    }

    #region Steam Server Callbacks
    private void SteamServer_OnSteamServersConnected()
    {
        Debug.Log("[SteamServer_OnSteamServersConnected] Success");
    }

    private void SteamServer_OnSteamServersDisconnected(Result result)
    {
        Debug.Log($"[SteamServer_OnSteamServersDisconnected] result: {result}");
    }

    private void SteamServer_OnSteamServerConnectFailure(Result result, bool stillTrying)
    {
        Debug.Log($"[SteamServer_OnSteamServerConnectFailure] result: {result}, stillTrying: {stillTrying}");
    }

    private void SteamServer_OnValidateAuthTicketResponse(SteamId steamID1, SteamId steamID2, AuthResponse authResponse)
    {
        Debug.Log($"[SteamServer_OnValidateAuthTicketResponse] steamID1: {steamID1}, steamID2: {steamID2}, authResponse: {authResponse}");
    }
    #endregion

}
