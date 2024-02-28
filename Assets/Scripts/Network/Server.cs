using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Server : NetworkBehaviour
{
    /// <summary>Singleton</summary>
    public static Server S { get; private set; }

    public MatchType MatchType;

    public List<Player> Players;

    protected virtual void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Player GetPlayer(byte clientID)
    {
        return Players.Find((p) => p.ClientID.Value == clientID);
    }

    public virtual void Init(MatchType matchType)
    {
        if (!Instance.S.IsServer) return;

        MatchType = matchType;

        NetworkManager.Singleton.OnServerStarted            += NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnServerStopped            += NetworkManager_OnServerStopped;
        NetworkManager.Singleton.OnTransportFailure         += NetworkManager_OnTransportFailure;
        NetworkManager.Singleton.OnClientConnectedCallback  += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    protected virtual void OnDisable()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnServerStarted            -= NetworkManager_OnServerStarted;
            NetworkManager.Singleton.OnServerStopped            -= NetworkManager_OnServerStopped;
            NetworkManager.Singleton.OnTransportFailure         -= NetworkManager_OnTransportFailure;
            NetworkManager.Singleton.OnClientConnectedCallback  -= NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;

            NetworkManager.Singleton.Shutdown();
        }
    }

    #region Network Manager Callbacks
    private void NetworkManager_OnServerStarted()
    {
        Debug.Log("[SERVER][NetworkManager_OnServerStarted]"); 
    }

    private void NetworkManager_OnServerStopped(bool stillTrying)
    {
        Debug.Log($"[SERVER][NetworkManager_OnServerStopped] Still trying: {stillTrying}");
    }

    private void NetworkManager_OnTransportFailure()
    {
        Debug.Log("[SERVER][NetworkManager_OnTransportFailure]");
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientID)
    {
        Debug.Log($"[SERVER][NetworkManager_OnClientConnectedCallback] ClientId {clientID}, Total Connected Clients: {NetworkManager.ConnectedClients.Count}");

        // @TODO: Make this dynamic depending on the type of match we're in

        if (NetworkManager.ConnectedClientsList.Count > 1)
        {
            var status = NetworkManager.SceneManager.LoadScene(Refs.SceneNames.PreloadMatch, UnityEngine.SceneManagement.LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogError("ERROR: Failed to load new scene");
            }
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        Debug.Log($"[SERVER][NetworkManager_OnClientDisconnectCallback] ClientId {clientID}, Total Connected Clients: {NetworkManager.ConnectedClients.Count}");
    }
    #endregion

}
