using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private Text RoomNameText;
    [SerializeField] private GameObject Player1Portrait;
    [SerializeField] private Text Player1NameText;
    [SerializeField] private GameObject Player2Portrait;
    [SerializeField] private Text Player2NameText;
    [SerializeField] private GameObject StartButton;
    private bool isInLobby;

    [SerializeField] private Text Player1NameInGameText;
    [SerializeField] private Text Player2NameInGameText;

    [SerializeField] private List<Transform> BoardPosition;

    [SerializeField] private GameObject MyPlayer;

    [SerializeField] private GameObject FinishPanel;
    [SerializeField] private Text WinnerNameText;
    private int winnerId;

    [SerializeField] private Text DiscMassageText;

    private GameObject SelectedPon;
    private bool isMasterClientTurn;

    private string PlayerName;
    
    struct BoardData
    {
        public int ownerId;
        public int sizeId;
    }
    BoardData[] boardDataArray;

    private void Awake()
    {
        // Room Owner Play first
        isMasterClientTurn = true;

        // Ui Management
        LobbyPanel.SetActive(true);
        Player1Portrait.SetActive(true);
        Player2Portrait.SetActive(false);
        StartButton.SetActive(false);
        isInLobby = true;
        FinishPanel.SetActive(false);

        RoomNameText.text = "Room Name : " + PhotonNetwork.room.name;
        PlayerName = PhotonNetwork.player.name;
        if (PhotonNetwork.isMasterClient)
        {
            Player1NameText.text = PlayerName;
        }

        // Set Board Array Data (Manual Constructor :v)
        boardDataArray = new BoardData[9];
        for(int i = 0; i < 9; i++)
        {
            boardDataArray[i].ownerId = 0;
            boardDataArray[i].sizeId = 0;
        }
    }

    void Start()
    {
        // Debugging
        //PhotonNetwork.Instantiate(MyPlayer.name, this.transform.position, Quaternion.identity, 0);
    }

   
    void Update()
    {
        // Check is another player is joined
        if (PhotonNetwork.playerList.Length >= 2 && isInLobby)
        {
            // Ui Portrait
            Player2Portrait.SetActive(true);
            // UI name
            if (PhotonNetwork.isMasterClient)
            {
                GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, Player1NameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName);
            }
            else
            {
                GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, Player2NameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName);
            }
            // Enable Start Button for room owner
            if (PhotonNetwork.isMasterClient)
            {
                StartButton.SetActive(true);
            }
        }
        else
        {
            Player2Portrait.SetActive(false);
            StartButton.SetActive(false);
        }

        // Just for UI
        if (!isInLobby)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (isMasterClientTurn)
                {
                    GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, Player1NameInGameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName + " Turn");
                }
                else
                {
                    GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, Player1NameInGameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName);
                }
            }
            if (!PhotonNetwork.isMasterClient)
            {
                if (isMasterClientTurn)
                {
                    GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, Player2NameInGameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName);
                }
                else
                {
                    GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, Player2NameInGameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName + " Turn");
                }
                Debug.Log("Player 2 Name Changed");
            }
        }
    }

    public void StartButtonMethod()
    {
        // This method is for Start button that appear for room owner
        // Close Waiting Panel
        GetComponent<PhotonView>().RPC("SetActiveObject", PhotonTargets.AllBuffered, LobbyPanel.transform.gameObject.GetComponent<PhotonView>().viewID, false);
        // Set Bool
        GetComponent<PhotonView>().RPC("SetIsInLobby", PhotonTargets.AllBuffered, false);
        // Create Player
        PhotonNetwork.Instantiate(MyPlayer.name, this.transform.position, Quaternion.identity, 0);
    }
    [PunRPC]
    void SetIsInLobby(bool set)
    {
        // This method is for turn Bool isInLobby, why?, idk
        isInLobby = set;
    }

    public void BackToMenuButtonMethod()
    {
        // Return to Menu Method and disconnec from server
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    void SetActiveObject(int theObject, bool setActive)
    {
        // Method to set Active Object Online
        PhotonView Temp = PhotonView.Find(theObject);
        Temp.transform.gameObject.SetActive(setActive);
    }
    [PunRPC]
    void ChangeTextName(int targetText, string text)
    {
        // Change text object (in this case is name) for both player
        PhotonView Temp = PhotonView.Find(targetText);
        Temp.GetComponent<Text>().text = text;
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        DiscMassageText.text = player.name + "Left The Game";
        Debug.Log(player.name + "Left The Game");
    }
    private void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        DiscMassageText.text = "";
        Debug.Log(player.name + "Join The Game");
    }

    public void LockSelectedPon(GameObject thePon)
    {
        // This method is for hold the selected Pon
        // Delete Pon before
        if(SelectedPon != null)
        {
            // Selected Pon Before
            SelectedPon.GetComponent<PonManager>().ResetSize();
            SelectedPon.GetComponent<PonManager>().SetLock(false);
        }
        // New Selected Pon
        SelectedPon = thePon;
        SelectedPon.GetComponent<PonManager>().SetLock(true);
    }

    public void BoardClicked(int boardId)
    {
        // Check if player turn
        if(isMasterClientTurn && PhotonNetwork.isMasterClient)
        {
            TurnProcess(boardId);
        }
        // Check if player-2 turn
        else if (!isMasterClientTurn && !PhotonNetwork.isMasterClient)
        {
            TurnProcess(boardId);
        }
    }
    private void TurnProcess(int boardId)
    {
        // This method is process for each player turn
        // Check if Pon is selected
        if (SelectedPon != null)
        {
            // Check if Pon Can be Placed
            if (boardDataArray[boardId].ownerId != PhotonNetwork.player.ID && boardDataArray[boardId].sizeId < SelectedPon.GetComponent<PonManager>().GetIdSize())
            {
                // Resize Pon (Syncronized)
                SelectedPon.GetComponent<PonManager>().ResetSize();
                // Move Pon (Syncronized)
                Vector3 PonNewPosition = new Vector3(BoardPosition[boardId].transform.position.x, BoardPosition[boardId].transform.position.y, SelectedPon.transform.position.z);
                SelectedPon.transform.position = PonNewPosition;
                // Change BoardData to All Player
                int number = boardId;
                int owner = SelectedPon.GetComponent<PonManager>().GetOwnerId();
                int size = SelectedPon.GetComponent<PonManager>().GetIdSize();
                GetComponent<PhotonView>().RPC("ChangeBoardData", PhotonTargets.AllBuffered, number, owner, size);
                // Reset SelectedPon data
                SelectedPon = null;
                // Change Turn
                GetComponent<PhotonView>().RPC("ChangeTurn", PhotonTargets.AllBuffered);
                // Check Board Data
                GetComponent<PhotonView>().RPC("CheckBoardData", PhotonTargets.AllBuffered);
            }
        }
    }

    [PunRPC]
    void ChangeTurn()
    {
        // This method is for changing both player turn data
        if (isMasterClientTurn)
        {
            isMasterClientTurn = false;
        }
        else
        {
            isMasterClientTurn = true;
        }
        Debug.Log("Turn Changed - " + isMasterClientTurn);
    }
    [PunRPC]
    void ChangeBoardData(int number, int owner, int size)
    {
        // This method is for changing Board data for both player
        boardDataArray[number].ownerId = owner;
        boardDataArray[number].sizeId = size;
    }
    [PunRPC]
    void CheckBoardData()
    {
        // Check Horizontal
        int count = 0;
        for(int i = 0; i < 3; i++)
        {
            if(boardDataArray[count].ownerId == boardDataArray[count + 1].ownerId && boardDataArray[count].ownerId == boardDataArray[count + 2].ownerId && boardDataArray[count].ownerId != 0)
            {
                winnerId = boardDataArray[count].ownerId;
                Debug.Log("Horizontal");
                GameOverMethod();
                return;
            }
            else
            {
                count += 3;
            }
        }

        // Check Vertical
        for (int i = 0; i < 3; i++)
        {
            if (boardDataArray[i].ownerId == boardDataArray[i + 3].ownerId && boardDataArray[i].ownerId == boardDataArray[i + 6].ownerId && boardDataArray[i].ownerId != 0)
            {
                winnerId = boardDataArray[i].ownerId;
                Debug.Log("Vertical");
                GameOverMethod();
                return;
            }
        }

        // Check Diagonal
        if (boardDataArray[0].ownerId == boardDataArray[4].ownerId && boardDataArray[0].ownerId == boardDataArray[8].ownerId && boardDataArray[0].ownerId != 0)
        {
            winnerId = boardDataArray[0].ownerId;
            Debug.Log("Diagonal");
            GameOverMethod();
            return;
        }
        if (boardDataArray[2].ownerId == boardDataArray[4].ownerId && boardDataArray[2].ownerId == boardDataArray[6].ownerId && boardDataArray[2].ownerId != 0)
        {
            winnerId = boardDataArray[2].ownerId;
            Debug.Log("Diagonal");
            GameOverMethod();
            return;
        }
    }

    void GameOverMethod()
    {
        Debug.Log("Game Over");
        FinishPanel.SetActive(true);
        if (PhotonNetwork.player.ID == winnerId)
        {
            GetComponent<PhotonView>().RPC("ChangeTextName", PhotonTargets.AllBuffered, WinnerNameText.gameObject.GetComponent<PhotonView>().viewID, PlayerName + " Win");
        }
    }
}
