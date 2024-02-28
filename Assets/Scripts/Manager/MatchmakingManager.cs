using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Steamworks.Data;

public class MatchmakingManager : MonoBehaviour
{

    /// <summary>Singleton</summary>
    public static MatchmakingManager S { get; private set; }

    [HideInInspector] public MatchType MatchType = MatchType.None;

    public Steamworks.ServerList.Internet CurrentRequest { get; private set; }

    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDisable()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        MatchType = MatchType.None;

        if (CurrentRequest != null)
        {
            CurrentRequest.OnResponsiveServer -= SteamworksServerList_OnResponsiveServer;
            CurrentRequest.OnChanges          -= SteamworksServerList_OnChanges;
            CurrentRequest.Dispose();
        }

        NetworkManager.Singleton?.Shutdown();

        Debug.Log("Disconnected");
    }

    public void RequestSteamGameServer(int maxMembers)
    {
        CurrentRequest = new Steamworks.ServerList.Internet();

        CurrentRequest.OnResponsiveServer += SteamworksServerList_OnResponsiveServer;
        CurrentRequest.OnChanges          += SteamworksServerList_OnChanges;

        CurrentRequest.RunQueryAsync(30);
    }

    private void SteamworksServerList_OnChanges()
    {
        Debug.Log($"[SteamworksServerList_OnChanges] Responsive: {CurrentRequest.Responsive.Count} - Unresponsive: {CurrentRequest.Unresponsive.Count}");
    }

    private void SteamworksServerList_OnResponsiveServer(ServerInfo serverInfo)
    {
        Debug.Log($"[SteamworksServerList_OnResponsiveServer] Found server '{serverInfo.Name}'");

        if (serverInfo.Players < serverInfo.MaxPlayers)
        {
            Debug.Log($"Joining server '{serverInfo.Name}'");

            CurrentRequest.OnResponsiveServer -= SteamworksServerList_OnResponsiveServer;
            CurrentRequest.OnChanges          -= SteamworksServerList_OnChanges;

            NetworkManager.Singleton.GetComponent<FacepunchTransport>().targetSteamId = serverInfo.SteamId;

            NetworkManager.Singleton.StartClient();
        }
    }

    public static int GetMaxMembersByMatchType(MatchType matchType)
    {
        switch (matchType)
        {
            case MatchType.OneVSOne:
                return 2;
            case MatchType.TwoVSTwo:
                return 4;
            case MatchType.ThreeVSThree:
                return 6;
        }
        return 0;
    }

    public static MatchType GetMatchTypeByMaxMembers(int maxMembers)
    {
        switch (maxMembers)
        {
            case 2:
                return MatchType.OneVSOne;
            case 4:
                return MatchType.TwoVSTwo;
            case 6:
                return MatchType.ThreeVSThree;
            default:
                return MatchType.None;
        }
    }
}

public enum MatchType
{
    None,
    OneVSOne,
    TwoVSTwo,
    ThreeVSThree,
}