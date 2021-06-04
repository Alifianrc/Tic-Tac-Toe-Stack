using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject LobbyPanel;

    [SerializeField] private InputField UsernameInput;
    [SerializeField] private GameObject PlayButton;

    [SerializeField] private Text PlayerNameText;
    [SerializeField] private GameObject JoinButton;
    [SerializeField] private InputField JoinInput;

    [SerializeField] private GameObject CreditPanel;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);

        // UI Management
        MainPanel.SetActive(true);
        PlayButton.SetActive(false);
        LobbyPanel.SetActive(false);
        JoinButton.SetActive(false);
        CreditPanel.SetActive(false);
    }

    private void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void ChangeUserNameInput()
    {
        if (UsernameInput.text.Length >= 3 && UsernameInput.text.Length <= 10)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
    }

    public void ChangeJoinInput()
    {
        if (JoinInput.text.Length >= 3 && JoinInput.text.Length <= 10)
        {
            JoinButton.SetActive(true);
        }
        else
        {
            JoinButton.SetActive(false);
        }
    }

    public void PlayButtonMethod()
    {
        PhotonNetwork.playerName = UsernameInput.text;
        PlayerNameText.text = "Hello " + PhotonNetwork.player.name;

        MainPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public void JoinGameButtonMethod()
    {
        PhotonNetwork.JoinOrCreateRoom(JoinInput.text, new RoomOptions() { maxPlayers = 2 }, null);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainScene");
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void CancelLobbyPanelButtonMathod()
    {
        MainPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }

    public void CreditOpenMethod()
    {
        CreditPanel.SetActive(true);
    }
    public void CreditCloseMethod()
    {
        CreditPanel.SetActive(false);
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
}
