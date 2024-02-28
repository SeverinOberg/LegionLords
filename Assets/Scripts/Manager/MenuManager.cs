using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class MenuManager : MonoBehaviour
{

    /// <summary>Singleton</summary>
    public static MenuManager S { get; private set; }

    private enum MenuType
    {
        Main,
        Play,
        Legion,
        Shop,
        BattlePass,
        Options,
    }

    [SerializeField] private GameObject[] _menus;
    [SerializeField] private GameObject _backButton;
    [SerializeField] private TextMeshProUGUI _menuTabTitleText;
    [SerializeField] private GameObject _matchmakingStatusBar;
    [SerializeField] private TextMeshProUGUI _matchmakingStatusText;
    [SerializeField] private GameObject _matchmakingCancelButton;

    private GameObject _currentMenuTab = null;

    private bool _isSearching;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _currentMenuTab = _menus[0];
    }

    public void FindMatch(int maxMembers)
    {
        if (_isSearching)
        {
            return;
        }

        MatchType matchType = MatchmakingManager.GetMatchTypeByMaxMembers(maxMembers);

        if (matchType == MatchType.None)
        {
            SetStatusText("something went wrong, try again");
            return;
        }

        MatchmakingManager.S.MatchType = matchType;

        _isSearching = true;

        NetworkManager.Singleton.NetworkConfig.NetworkTransport = FindAnyObjectByType<FacepunchTransport>();

        _matchmakingStatusBar.SetActive(true);

        SetStatusText("searching...");

        MatchmakingManager.S.RequestSteamGameServer(maxMembers);
    }

    public void CancelMatchmaking()
    {
        StopAllCoroutines();
        MatchmakingManager.S.Disconnect();
        SetStatusText("");
        ShowMatchmakingQueue(false);
        _isSearching = false;
    }

    public void ShowMatchmakingQueue(bool show = true)
    {
        if (show)
        {
            _matchmakingStatusBar.SetActive(true);
        }
        else
        {
            _matchmakingStatusBar.SetActive(false);
        }
    }

    private void OpenMenu(MenuType menuType)
    {
        int menuIndex = (int)menuType;

        _currentMenuTab.SetActive(false);
        _menus[menuIndex].SetActive(true);

        _currentMenuTab = _menus[menuIndex];

        if (menuType == MenuType.Main)
        {
            _backButton.SetActive(false);
            _menuTabTitleText.text = "";
        }
        else
        {
            _backButton.SetActive(true);
            _menuTabTitleText.text = menuType.ToString();
        }
    }

    public void OnClickPlay()
    {
        OpenMenu(MenuType.Play);
    }

    public void OnClickLegion()
    {
        Client.S.LegionManager.UpdateLegion();
        OpenMenu(MenuType.Legion);
    }

    public void OnClickShop()
    {
        OpenMenu(MenuType.Shop);
    }

    public void OnClickBattlePass()
    {
        OpenMenu(MenuType.BattlePass);
    }

    public void OnClickOptions()
    {
        OpenMenu(MenuType.Options);
    }
    
    public void OnClickBack()
    {
        OpenMenu(MenuType.Main);
    }

    private void SetStatusText(string text)
    {
        _matchmakingStatusText.text = text;
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    MatchmakingManager.S.Disconnect();
                    Application.Quit();
        #endif
    }

    #region Development
        public void OnClickStartServer()
        {
            Debug.Log("@---| INITIALIZING DEVELOPMENT SERVER |---@");

            MatchmakingManager.S.MatchType = MatchType.OneVSOne;

            NetworkManager.Singleton.GetComponent<UnityTransport>().enabled = true;
            NetworkManager.Singleton.GetComponent<FacepunchTransport>().enabled = false;

            NetworkManager.Singleton.NetworkConfig.NetworkTransport = FindAnyObjectByType<UnityTransport>();
        
            if (!NetworkManager.Singleton.StartServer())
            {
                Debug.LogError("ERROR: NetworkManager failed to start server");
            }
        }

        public void OnClickStartHost()
        {
            Debug.Log("@---| INITIALIZING DEVELOPMENT HOST |---@");

            MatchmakingManager.S.MatchType = MatchType.OneVSOne;

            NetworkManager.Singleton.GetComponent<UnityTransport>().enabled = true;
            NetworkManager.Singleton.GetComponent<FacepunchTransport>().enabled = false;

            NetworkManager.Singleton.NetworkConfig.NetworkTransport = FindAnyObjectByType<UnityTransport>();
        
            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogError("ERROR: NetworkManager failed to start host");
            }
        }

        public void OnClickStartClient()
        {
            Debug.Log("@---| INITIALIZING DEVELOPMENT CLIENT |---@");

            MatchmakingManager.S.MatchType = MatchType.OneVSOne;

            NetworkManager.Singleton.GetComponent<UnityTransport>().enabled = true;
            NetworkManager.Singleton.GetComponent<FacepunchTransport>().enabled = false;

            NetworkManager.Singleton.NetworkConfig.NetworkTransport = FindAnyObjectByType<UnityTransport>();
        
            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogError("ERROR: NetworkManager failed to start client");
            }
        }
    #endregion

}
