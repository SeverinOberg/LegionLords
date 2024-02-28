using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public static UIManager Singleton;

    [SerializeField] private GameObject _canvasUI;
    [SerializeField] private GameObject _menuUI;
    [SerializeField] private GameObject _optionsUI;
    [SerializeField] private GameObject _spellsMenuUI;
    [SerializeField] private GameObject _victoryUI;
    [SerializeField] private GameObject _defeatUI;

    private List<GameObject> _allUI = new();

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Transform child in _canvasUI.transform)
        {
            _allUI.Add(child.gameObject);
        }
        
    }

    public void DeactivateAllUI()
    {
        foreach (var ui in _allUI)
        {
            ui.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        if (_optionsUI.activeSelf)
        {
            _optionsUI.SetActive(false);
            return;
        }
        _menuUI.SetActive(!_menuUI.activeSelf);
    }

    public void OnClickExitMenu()
    {
        _menuUI.SetActive(false);
        _optionsUI.SetActive(false);
    }

    
    public void ReturnToExitMenu()
    {
        _optionsUI.SetActive(false);

        _menuUI.SetActive(true);
    }

    public void OnClickQuitMatch()
    {
        DeactivateAllUI();
        MatchManager.S.OnGameOverServerRpc(Player.S.Team.Value);
    }

    public void ActivateVictoryUI()
    {
        DeactivateAllUI();
        _victoryUI.SetActive(true);
    }

    public void ActivateDefeatUI()
    {
        DeactivateAllUI();
        _defeatUI.SetActive(true);
    }

    public void DeactivateSpellsMenuUI()
    {
        _spellsMenuUI.SetActive(false);
    }

    public void OnClickExitEndMatch()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(Refs.SceneNames.Menu, LoadSceneMode.Single);
    }

    public void OnClickOptions()
    {
        _menuUI.SetActive(false);
        _optionsUI.SetActive(true);
    }

    public void Exit()
    {
        NetworkManager.Singleton.Shutdown();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
