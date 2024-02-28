using UnityEngine;
using Steamworks;
using Unity.Netcode;

public class Client : MonoBehaviour
{

    /// <summary>Singleton</summary>
    public static Client S { get; private set; }

    public SteamId       SteamID      { get; private set; }
    public int           AccountID    { get; private set; }
    public string        SteamName    { get; private set; }

    public LegionManager LegionManager { get; private set; } = new();

    public void Init(int accountID, SteamId steamID, string steamName)
    {
        AccountID  = accountID;
        SteamID    = steamID;
        SteamName  = steamName;
    }

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

    public void Init()
    {
        NetworkManager.Singleton.OnClientStarted            += NetworkManager_OnClientStarted;
        NetworkManager.Singleton.OnClientStopped            += NetworkManager_OnClientStopped;
        NetworkManager.Singleton.OnTransportFailure         += NetworkManager_OnTransportFailure;
        NetworkManager.Singleton.OnClientConnectedCallback  += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void OnDisable()
    {
        if (!NetworkManager.Singleton) return;

        NetworkManager.Singleton.OnClientStarted            -= NetworkManager_OnClientStarted;
        NetworkManager.Singleton.OnClientStopped            -= NetworkManager_OnClientStopped;
        NetworkManager.Singleton.OnTransportFailure         -= NetworkManager_OnTransportFailure;
        NetworkManager.Singleton.OnClientConnectedCallback  -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;

        NetworkManager.Singleton.Shutdown();
    }


    private void NetworkManager_OnClientStarted()
    {
        Debug.Log("[CLIENT][NetworkManager_OnClientStarted]"); 
    }

    private void NetworkManager_OnClientStopped(bool stillTrying)
    {
        Debug.Log($"[CLIENT][NetworkManager_OnClientStopped] Still trying: {stillTrying}");
    }

    private void NetworkManager_OnTransportFailure()
    {
        Debug.Log("[CLIENT][NetworkManager_OnTransportFailure]");
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientID)
    {
        Debug.Log($"[CLIENT][NetworkManager_OnClientConnectedCallback] ClientId {clientID}");
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        Debug.Log($"[CLIENT][NetworkManager_OnClientDisconnectCallback] ClientId {clientID}");
    }

}