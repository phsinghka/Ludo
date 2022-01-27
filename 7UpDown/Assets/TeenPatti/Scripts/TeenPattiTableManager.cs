using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class PlayersData{
    public string name;
    public int avatarId;
    public float playPoint;
    public bool isOccupied;
    public int actorNumber;
    public bool isLocal;
    public int[] cards = new int[TPConstants.NUM_OF_CARDS];
    public double cardScores;
    public int blindCount;
    public TeenPatti.PlayerCardStatus playerCardStatus;
    public TeenPatti.PlayerLastPlay playerLastPlay;
    public TeenPatti.PlayerPosition assignedPos;
    public TeenPatti.PlayerStatus playerStatus;


}
[System.Serializable]
public class RoomData{

    public float BootValue;
    public float ChalLimit;
    public float PotLimit;
    public float currentChalValue;
    public int currentPlayerTurnIndex;
    public float watingTime;
    public int jokerValue;
    public float OnTableMoney;
    public TeenPatti.RoomStatus roomStatus;
    public TeenPatti.RoomType roomType;
    public int[] playersTurns;
    public float playersTurnsTimer;

}

public class TeenPattiTableManager : MonoBehaviourPunCallbacks
{
    public static TeenPattiTableManager Instance;
    public PhotonView PhotonView;
    public PlayersData[] playersData = new PlayersData[TPConstants.MAX_PLAYERS];
    public int localPlayerIndex=0;
    public RoomData roomData;
    public Text timerText;
    public Text chalIndex;
    public Text playerTurns1;
    public Text playerTurns2;
    public Text playerTurns3;
    public Text playerTurns4;
    public Text playerTurns5;
    public Text chaalCurrnIndex;
    public GameObject InfoGameObject;

    public bool start;
    public bool playerTakingTurn;
    void Awake(){
        Instance = this;
        PhotonView = GetComponent<PhotonView>();

    }
    void Start(){
        start = false;
        playerTakingTurn = false;
        if(PhotonNetwork.IsMasterClient){
            SetupRoomFirstTime();
            AddPlayer(PhotonNetwork.LocalPlayer);
        }
        if(PhotonNetwork.IsConnected){
            PhotonConnectionLogin.Instance.MasterDisconnected.SetActive(false);
        }
        else{
            PhotonConnectionLogin.Instance.MasterDisconnected.SetActive(false);
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount <=1){
            StartCoroutine(ShowHidePanel(5));
        }
        else{
            StartCoroutine(ShowHidePanel(1));
        }
        SetInfoValues();

        

        // chalIndex.text = "ChalIndex =";
    }
    public void SetInfoValues(){
        TeenPatti.RoomType GameName = (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE];

        if(GameName == TeenPatti.RoomType.NO_VARIATION){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - Classic";
        }
        else if(GameName == TeenPatti.RoomType.AK47){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - AK47";
        }
        else if(GameName == TeenPatti.RoomType.JOKER){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - Joker";
        }
        else if(GameName == TeenPatti.RoomType.LOWER_JOKER){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - Lowest Joker";
        }
        else if(GameName == TeenPatti.RoomType.HIGHEST_JOKER){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - Highest Joker";
        }
        else if(GameName == TeenPatti.RoomType.FOURXBOOT){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - 4XBoot";
        }
        else if(GameName == TeenPatti.RoomType.MUFLIS){
            InfoGameObject.transform.Find("TableType").GetComponentInChildren<Text>().text = "TeenPatti - Muflish";
        }
        InfoGameObject.transform.Find("TableBootValue").GetComponentInChildren<Text>().text = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE] +"/Boot";
        InfoGameObject.transform.Find("TableID").GetComponentInChildren<Text>().text = "Table ID : "+PhotonNetwork.CurrentRoom.Name;
    }
    IEnumerator ShowHidePanel(int time){
        PhotonConnectionLogin.Instance.WaitForRoom.SetActive(true);
        yield return new WaitForSeconds(time);
        PhotonConnectionLogin.Instance.WaitForRoom.SetActive(false);

    }
    void Update(){
        if(start && PhotonNetwork.CurrentRoom.PlayerCount>1){
            //Debug.LogError("STARTCALLED");
            if(roomData.watingTime>=0){
                roomData.watingTime -= Time.deltaTime;
                timerText.text = "Game Starts in "+(int)roomData.watingTime;
            }
            else if(PhotonNetwork.IsMasterClient){
                timerText.text = "";
                start = false;
                Debug.LogError("RESTART GAME 0 "+(int)roomData.roomType);
                if(roomData.roomStatus != TeenPatti.RoomStatus.INGAME){
                    StartGameAfterWaiting();
                    Debug.LogError("RESTART GAME 1 "+(int)roomData.roomType);
                }
            }
            
        }
        else{
            timerText.text = "Waiting For other players to Join !!";
        }
        if(roomData.playersTurns.Length == 1){
            playerTurns1.text = roomData.playersTurns[0]+"";
            playerTurns2.text = "";
            playerTurns3.text = "";
            playerTurns4.text = "";
            playerTurns5.text = "";
        }
        else if (roomData.playersTurns.Length == 2){
            playerTurns1.text = roomData.playersTurns[0]+"";
            playerTurns2.text = roomData.playersTurns[1]+"";
            playerTurns3.text = "";
            playerTurns4.text = "";
            playerTurns5.text = "";
        }
        else if (roomData.playersTurns.Length == 3){
            playerTurns1.text = roomData.playersTurns[0]+"";
            playerTurns2.text = roomData.playersTurns[1]+"";
            playerTurns3.text = roomData.playersTurns[2]+"";
            playerTurns4.text = "";
            playerTurns5.text = "";
        }
        else if (roomData.playersTurns.Length == 4){
            playerTurns1.text = roomData.playersTurns[0]+"";
            playerTurns2.text = roomData.playersTurns[1]+"";
            playerTurns3.text = roomData.playersTurns[2]+"";
            playerTurns4.text = roomData.playersTurns[3]+""; 
            playerTurns5.text = "";
        }
        else if (roomData.playersTurns.Length == 5){
            playerTurns1.text = roomData.playersTurns[0]+"";
            playerTurns2.text = roomData.playersTurns[1]+"";
            playerTurns3.text = roomData.playersTurns[2]+"";
            playerTurns4.text = roomData.playersTurns[3]+"";
            playerTurns5.text = roomData.playersTurns[4]+"";
        }
        

        chaalCurrnIndex.text = roomData.currentPlayerTurnIndex+"";
        if(roomData.OnTableMoney>0){
            chalIndex.text = ""+System.Math.Round( roomData.OnTableMoney,2) + " ₹";
        }
        else{
            chalIndex.text = "";
        }
        if(playerTakingTurn){
            if(roomData.playersTurnsTimer>=0){
                roomData.playersTurnsTimer -= Time.deltaTime;
                timerText.text = "";
                TeenPattiGameManager.Instance.UpdateTimerTime(roomData.playersTurnsTimer);
            }
            else if(PhotonNetwork.IsMasterClient){
                playerTakingTurn = false;
                Debug.LogError("PACKS "+roomData.playersTurns[roomData.currentPlayerTurnIndex]);
                int value = System.Array.IndexOf(TablePosition,roomData.playersTurns[roomData.currentPlayerTurnIndex]);
                PlayerPackCards(value);
            }
        }
    }
    // public void OnApplicationQuit()     
    // {
    //     PhotonView.RPC("LetsSee", RpcTarget.All, 2);
    //     LeaveRoom();
    // }
    
    public void OnApplicationPause(){
        #if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android && PhotonNetwork.IsMasterClient){
            PhotonNetwork.Disconnect();
        }
        #endif
    }
    public void OnApplicationFocus(){
        #if UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android)
        {
            PhotonView.RPC("LetsSee", RpcTarget.All, 1);  
        }
        #endif
    }
    [PunRPC] public void LetsSee(int value){
        if(value == 0){
            Debug.LogError("PAUSE");
        }
        if(value == 1){
            Debug.LogError("FOCUS");
            if(PhotonNetwork.IsMasterClient){
                SendOnJoinOnLeaveRPC(-1);
            }
        }
        if(value == 2){
            Debug.LogError("QUIT");
        }
    }
    #region  Player Connection
    string[] tempname = new string[TPConstants.MAX_PLAYERS];
    int[] tempavatarId = new int[TPConstants.MAX_PLAYERS];
    float[] tempplayPoint = new float[TPConstants.MAX_PLAYERS];
    int[] tempPosindex = new int[TPConstants.MAX_PLAYERS];
    int[] tempPlayerCards1 = new int[TPConstants.MAX_PLAYERS];
    int[] tempPlayerCards2 = new int[TPConstants.MAX_PLAYERS];
    int[] tempPlayerCards3 = new int[TPConstants.MAX_PLAYERS];
    double[] tempPlayerCardScores = new double[TPConstants.MAX_PLAYERS];
    int[] tempBlindCount = new int[TPConstants.MAX_PLAYERS];
    int[] tempCardStatus = new int[TPConstants.MAX_PLAYERS];

    bool[] tempisOccupied = new bool[TPConstants.MAX_PLAYERS];
    int[] tempplayerStatus = new int[TPConstants.MAX_PLAYERS];
    int[] tempactorNumber = new int[TPConstants.MAX_PLAYERS];
    bool[] tempisLocal = new bool[TPConstants.MAX_PLAYERS];
    public int[] TablePosition = new int[TPConstants.MAX_PLAYERS];
    public GameObject[] PlayerList;

    public void LeaveRoom(){
        Debug.LogError("LEFT ROOM");
        // if(roomData.playersTurns[roomData.currentPlayerTurnIndex]==0){
        //     PhotonView.RPC("ActionPlayerPacks", RpcTarget.All, localPlayerIndex);

        // }
        // else{
        //     PhotonView.RPC("ActionPlayerLeaves", RpcTarget.All, localPlayerIndex);
        // }
        PhotonConnectionLogin.Instance.isSwitch = false;
        PhotonNetwork.LeaveRoom(false);
    }
    public void SwitchTable(){
        PhotonNetwork.Disconnect();
        // PhotonNetwork.LeaveRoom();
    }
    public void AddPlayer(Player player){
        int index = 0;
        for(int i = 0; i < playersData.Length;i++){
            if(!playersData[i].isOccupied){
                playersData[i].cards = new int[TPConstants.NUM_OF_CARDS];
                Debug.Log((string)player.CustomProperties[TPConstants.PLAYER_NAME]);
                playersData[i].name = (string)player.CustomProperties[TPConstants.PLAYER_NAME];
                playersData[i].avatarId = (int)player.CustomProperties[TPConstants.AVATAR_ID];
                playersData[i].playPoint = (float)player.CustomProperties[TPConstants.PLAY_POINT];
                playersData[i].assignedPos = (TeenPatti.PlayerPosition)i;
                playersData[i].isOccupied = true;
                playersData[i].actorNumber = player.ActorNumber;
                playersData[i].isLocal = false;
                playersData[i].blindCount = 4;
                playersData[i].cardScores = -1;
                for (int j = 0; j < playersData[i].cards.Length; j++)
                {
                    playersData[i].cards[j] = -1;
                }
                playersData[i].playerCardStatus = TeenPatti.PlayerCardStatus.BLIND;
                playersData[i].playerStatus = TeenPatti.PlayerStatus.WAITING;
                index = i;
                break;
            }
        }
        SendOnJoinOnLeaveRPC(index);
    }
    public void RejoinedPlayer(Player player){
        // int index = 0;
        // for(int i = 0; i < playersData.Length;i++){
        //     if(playersData[i].actorNumber == player.ActorNumber){
        //         playersData[i].cards = new int[TPConstants.NUM_OF_CARDS];
        //         Debug.Log((string)player.CustomProperties[TPConstants.PLAYER_NAME]);
        //         playersData[i].name = (string)player.CustomProperties[TPConstants.PLAYER_NAME];
        //         playersData[i].avatarId = (int)player.CustomProperties[TPConstants.AVATAR_ID];
        //         playersData[i].playPoint = (float)player.CustomProperties[TPConstants.PLAY_POINT];
        //         playersData[i].assignedPos = (TeenPatti.PlayerPosition)i;
        //         playersData[i].isOccupied = true;
        //         playersData[i].actorNumber = player.ActorNumber;
        //         playersData[i].isLocal = false;
        //         playersData[i].blindCount = 4;
        //         playersData[i].cardScores = -1;
        //         for (int j = 0; j < playersData[i].cards.Length; j++)
        //         {
        //             playersData[i].cards[j] = -1;
        //         }
        //         playersData[i].playerCardStatus = TeenPatti.PlayerCardStatus.BLIND;
        //         playersData[i].playerStatus = TeenPatti.PlayerStatus.WAITING;
        //         index = i;
        //         break;
        //     }
        // }
        SendOnJoinOnLeaveRPC(-1);
    }
    public void RemovePlayer(Player player){
        int index = 0;
        for(int i = 0; i < playersData.Length;i++){
            if(playersData[i].actorNumber == player.ActorNumber){
                playersData[i].cards = new int[TPConstants.NUM_OF_CARDS];
                playersData[i].name = "";
                playersData[i].avatarId = 0;
                playersData[i].playPoint = 0;
                playersData[i].isOccupied = false;
                playersData[i].actorNumber = 0;
                playersData[i].isLocal = player.IsLocal;
                playersData[i].blindCount = 4;
                playersData[i].cardScores = -1;
                
                for (int j = 0; j < playersData[i].cards.Length; j++)
                {
                    playersData[i].cards[j] = -1;
                }
                playersData[i].playerCardStatus = TeenPatti.PlayerCardStatus.BLIND;
                playersData[i].playerStatus = TeenPatti.PlayerStatus.WAITING;
                index = i;
                break;

            }
        }
        SendOnJoinOnLeaveRPC(index);
    }
    public void SendOnJoinOnLeaveRPC(int index){
        //PLAYER DATA
        for(int i = 0; i < playersData.Length;i++){
            if(playersData[i].cards.Length!=3){
                playersData[i].cards = new int[TPConstants.NUM_OF_CARDS];
            }
            tempname[i] = playersData[i].name;
            tempavatarId [i] = playersData[i].avatarId;
            tempplayPoint[i] = playersData[i].playPoint;
            tempisOccupied [i] = playersData[i].isOccupied;
            tempactorNumber[i] = playersData[i].actorNumber;
            tempPlayerCards1[i] = playersData[i].cards[0];
            tempPlayerCards2[i] = playersData[i].cards[1];
            tempPlayerCards3[i] = playersData[i].cards[2];
            tempPlayerCardScores[i] = playersData[i].cardScores;
            tempBlindCount[i] = playersData[i].blindCount;
            tempCardStatus[i] = (int)playersData[i].playerCardStatus;
            tempPosindex[i] = (int)playersData[i].assignedPos;
            tempplayerStatus[i] = (int)playersData[i].playerStatus;
        }

        //ROOM DATA
        tempcurrentChalValue = roomData.currentChalValue;
        tempCurrChalIndex = roomData.currentPlayerTurnIndex;
        tempwatingTime = roomData.watingTime;
        tempOnTableMoney = roomData.OnTableMoney;
        temproomStatus = (int)roomData.roomStatus;
        tempplayersTurns = new int[roomData.playersTurns.Length];
        // int startIndex = System.Array.IndexOf(TablePosition,0);
        for (int i = 0; i <tempplayersTurns.Length; i++){
            int value = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
            tempplayersTurns[i] = value;
            Debug.LogError(tempplayersTurns[i] +" MASTER: " +value);
        }
        tempplayersTurnsTimer = roomData.playersTurnsTimer;

                                                        //PLAYERS DATA
        PhotonView.RPC("UpdatePlayerList",RpcTarget.All,index,
                                                        tempname,
                                                        tempavatarId,
                                                        tempplayPoint,
                                                        tempisOccupied,
                                                        tempactorNumber,
                                                        tempPlayerCards1,
                                                        tempPlayerCards2,
                                                        tempPlayerCards3,
                                                        tempPlayerCardScores,
                                                        tempBlindCount,
                                                        tempCardStatus,
                                                        tempPosindex,
                                                        tempplayerStatus,
                                                        //ROOM DATA
                                                        tempcurrentChalValue,
                                                        tempCurrChalIndex,
                                                        tempwatingTime,
                                                        tempOnTableMoney,
                                                        temproomStatus,
                                                        tempplayersTurns,
                                                        tempplayersTurnsTimer,
                                                        roomData.jokerValue);
    }
    [PunRPC] public void UpdatePlayerList(int newIndex, string[] name,int[] avatarId,float[] playPoint,bool[] isOccupied,int[] actorNumber,int[] playerCards1, int[] playerCards2, int[] playerCards3, double[] playerCardsScores,int[] blindCount, int[] playerCardsStatus, int[] posIndex, int[] playerStatus,//Player Properties
    float currentChalValue, int currplayerIndex, float watingTime, float OnTableMoney,int roomStatus, int[] player_turns, float playerTurnTimer,int jokerValue//Room Properties
    ){

        //ROOM DATA
        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.FOURXBOOT){
            roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*4;
        }
        else{
            roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE];
        }
        roomData.ChalLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*128;
        roomData.PotLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*1024;
        roomData.currentChalValue = currentChalValue;
        roomData.currentPlayerTurnIndex = currplayerIndex;
        roomData.watingTime = watingTime;

        roomData.OnTableMoney = OnTableMoney;
        roomData.roomStatus = (TeenPatti.RoomStatus)roomStatus;

        roomData.roomType = (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE];

        roomData.playersTurns = new int[player_turns.Length];
        roomData.playersTurnsTimer = playerTurnTimer;
        roomData.jokerValue = jokerValue;
        
        for(int i = 0; i < playersData.Length;i++){
            if(PhotonNetwork.LocalPlayer.ActorNumber == actorNumber[i]){
                playersData[i].isLocal = true;
                localPlayerIndex = i;
                for(int j = 0; j < playersData.Length;j++){

                    if(j-i>=0){
                        TablePosition[j] = j-i;
                    }
                    else{
                        TablePosition[j] = j-i+TPConstants.MAX_PLAYERS;
                    }
                }
            }
        }
        
        for (int i = 0; i <player_turns.Length; i++){
            roomData.playersTurns[i] = TablePosition[player_turns[i]];
            Debug.LogError(TablePosition[i]+" POSSS "+roomData.playersTurns[i]+" POSSS "+player_turns[i]);
        }
        AssignCardAnimation(-1);
        //PLAYERS DATA
        for(int i = 0; i < playersData.Length;i++){
            if(playersData[i].cards.Length!=3){
                playersData[i].cards = new int[TPConstants.NUM_OF_CARDS];
            }
            playersData[i].name= name[i];
            playersData[i].avatarId= avatarId[i];
            playersData[i].playPoint= playPoint[i];
            playersData[i].isOccupied= isOccupied[i];
            playersData[i].actorNumber= actorNumber[i];
            playersData[i].cards[0] = playerCards1[i];
            playersData[i].cards[1] = playerCards2[i];
            playersData[i].cards[2] = playerCards3[i];
            playersData[i].cardScores = playerCardsScores[i];
            playersData[i].blindCount = blindCount[i];
            playersData[i].playerCardStatus = (TeenPatti.PlayerCardStatus)playerCardsStatus[i];
            playersData[i].assignedPos= (TeenPatti.PlayerPosition)TablePosition[i];
            //Debug.LogError("NEW INDEX "+newIndex);
            if(i!=newIndex){
                playersData[i].playerStatus = (TeenPatti.PlayerStatus)playerStatus[i];
            }
            else{
                if(roomData.roomStatus == TeenPatti.RoomStatus.WAITING){
                    playersData[i].playerStatus = TeenPatti.PlayerStatus.READY;
                }
                else if(roomData.roomStatus == TeenPatti.RoomStatus.READY) {
                    playersData[i].playerStatus = TeenPatti.PlayerStatus.READY;
                }
                else if(roomData.roomStatus == TeenPatti.RoomStatus.INGAME){
                    playersData[i].playerStatus = TeenPatti.PlayerStatus.WAITING;
                }
            }

            //DIPLAYING PLAYERS
            if(playersData[i].isOccupied){
                PlayerList[TablePosition[i]].SetActive(true);
                AssignCardAnimation(TablePosition[i]);
                PlayerList[TablePosition[i]].transform.Find("PlayerName").GetComponentInChildren<Text>().text = playersData[i].name;
                PlayerList[TablePosition[i]].transform.Find("PlayerTotalAmount").GetComponentInChildren<Text>().text =""+ playersData[i].playPoint;
                PlayerList[TablePosition[i]].transform.Find("PlayerImage").GetComponentInChildren<Image>().sprite = MainDataManager.Instance.avatars[ playersData[i].avatarId];
                if(playersData[i].playerStatus == TeenPatti.PlayerStatus.WAITING){
                    PlayerList[TablePosition[i]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(1,0,0,1); 
                }
                else if (playersData[i].playerStatus == TeenPatti.PlayerStatus.READY){
                    PlayerList[TablePosition[i]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,1,0,1);
                }
            }
            else{
                PlayerList[TablePosition[i]].SetActive(false);
            }
        }

        
        if(PhotonNetwork.CurrentRoom.PlayerCount>=2){
                if(playersData[localPlayerIndex].playerStatus != TeenPatti.PlayerStatus.WAITING){
                    if(roomData.roomStatus!=TeenPatti.RoomStatus.INGAME){
                        roomData.watingTime = TPConstants.PLAYER_WAITING_TIME;
                        start = true;
                    }
                }
                else{
                    start = false;
                    timerText.text = "Wait for next Round to start";
                }
            
            // if(PhotonNetwork.IsMasterClient){
            //     PhotonView.RPC("StartRoomGame", RpcTarget.All);
            // }
        }
        else{
            //TODO: RESET ROOM DATA 
        }

        Debug.Log("I GOT IT");
        if(roomData.roomStatus == TeenPatti.RoomStatus.INGAME){
            SetupCurrentScene();
        }

    }
    public void SetupCurrentScene(){
        if(roomData.playersTurns[roomData.currentPlayerTurnIndex]==0){
            TeenPattiActionPanel.Instance.UpdateActionPanelValues();
            actionPanel.SetActive(true);
        }
        else{
            actionPanel.SetActive(false);
        }
        JokerObject.SetActive(false);
        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.JOKER){
            JokerObject.transform.Find("Image").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[roomData.jokerValue];
            JokerObject.SetActive(true);
        }
        TeenPattiGameManager.Instance.ActivateTimerObject(roomData.playersTurns[roomData.currentPlayerTurnIndex]);
        playerTakingTurn = true;
        int index = System.Array.IndexOf(roomData.playersTurns,0);
        for (int i = 0; i < roomData.playersTurns.Length; i++)
        {
            for (int j = 0; j <TPConstants.NUM_OF_CARDS; j++){
                int value = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
                PlayerList[roomData.playersTurns[i]].transform.Find("VariationCards").Find(j.ToString()).GetComponentInChildren<Image>().color = new Color(1,1,1,1);
                PlayerList[roomData.playersTurns[i]].transform.Find("VariationCards").Find(j.ToString()).GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiScoreCalc.Instance.GetCardsOnVariation(playersData[value].cards, roomData.jokerValue, (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE])[j]];
                if(playersData[value].cards == TeenPattiScoreCalc.Instance.GetCardsOnVariation(playersData[value].cards, roomData.jokerValue, (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE])){
                    PlayerList[roomData.playersTurns[i]].transform.Find("VariationCards").Find(j.ToString()).GetComponentInChildren<Image>().color = new Color(1,1,1,0);
                }
            }
            if(i!=index){

                PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("0").gameObject.SetActive(true);
                PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("1").gameObject.SetActive(true);
                PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("2").gameObject.SetActive(true);
                PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("CardStatus").gameObject.SetActive(true);
                int tempIndex = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
                Debug.LogError(roomData.playersTurns[i]+" ROOM DATA "+tempIndex);
                if(playersData[tempIndex].playerCardStatus == TeenPatti.PlayerCardStatus.SEEN){
                    PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("CardStatus").Find("Status").GetComponentInChildren<Text>().text = "Seen";
                }
                else{
                    PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("CardStatus").Find("Status").GetComponentInChildren<Text>().text = "Blind";
                }
                // PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("0").gameObject.SetActive(true);
                // PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("1").gameObject.SetActive(true);
                // PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("2").gameObject.SetActive(true);
            }
        }
        if(index!=-1){
            PlayerList[0].transform.Find("Cards").Find("0").gameObject.SetActive(true);
            PlayerList[0].transform.Find("Cards").Find("1").gameObject.SetActive(true);
            PlayerList[0].transform.Find("Cards").Find("2").gameObject.SetActive(true);
            if(playersData[localPlayerIndex].playerCardStatus == TeenPatti.PlayerCardStatus.SEEN){
                TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiTableManager.Instance.playersData[localPlayerIndex].cards[0]];
                TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiTableManager.Instance.playersData[localPlayerIndex].cards[1]];
                TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiTableManager.Instance.playersData[localPlayerIndex].cards[2]];
            }
            else{
                TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
                TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
                TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
                TeenPattiGameManager.Instance.SeeBtn.SetActive(true);
            }
        }
        else{
            PlayerList[0].transform.Find("Cards").Find("0").gameObject.SetActive(false);
            PlayerList[0].transform.Find("Cards").Find("1").gameObject.SetActive(false);
            PlayerList[0].transform.Find("Cards").Find("2").gameObject.SetActive(false);
            TeenPattiGameManager.Instance.SeeBtn.SetActive(false);

        }
        


    }
    #endregion

    #region  Room Status
    float tempOnTableMoney;
    float tempcurrentChalValue;
    float tempwatingTime;
    int temproomStatus;
    float tempplayersTurnsTimer;
    int tempCurrChalIndex;
    int[] tempplayersTurns;
    public GameObject actionPanel;
    public GameObject JokerObject;
    public GameObject SideShowAnimObject;
    
    public void SetupRoomFirstTime(){
        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.FOURXBOOT){
            roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*4;
        }
        else{
            roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE];
        }
        roomData.roomType = (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE];
        roomData.ChalLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*128;
        roomData.PotLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*1024;
        roomData.OnTableMoney = 0;
        roomData.roomStatus = TeenPatti.RoomStatus.WAITING;
        roomData.currentChalValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE];
        roomData.watingTime = TPConstants.PLAYER_WAITING_TIME;
        roomData.playersTurnsTimer = TPConstants.PLAYER_TURN_WATING_TIME;
        timerText.text = "Waiting For other players to Join !!";
    }
    public void StartGameAfterWaiting(){
        int count = 0;
        int jokerValue = -1;
        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.JOKER){
            jokerValue = Random.Range(0,TPConstants.CARDS_COUNT);
        }

        for(int i = 0; i < playersData.Length;i++){
            if(playersData[i].playerStatus != TeenPatti.PlayerStatus.WAITING && playersData[i].isOccupied){
                count++;
            }
        } 
        Debug.LogError("RESTART GAME 1");
        int startIndex = Random.Range(0,count); 
        // startIndex = 1; 

        int[] tempCards = new int[TPConstants.NUM_OF_CARDS*count];
        tempCards = TeenPattiScoreCalc.Instance.DistributeCards(count);

        double[] tempCardScores = new double[count];
        for (int i = 0; i <tempCardScores.Length; i++){
            int temp = i*TPConstants.NUM_OF_CARDS;
            int[] cards = {tempCards[temp],tempCards[temp+1],tempCards[temp+2]};
            tempCardScores[i] = TeenPattiScoreCalc.Instance.GetCardsScoreOnVariation(cards, jokerValue, (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE]);
        }

        int[] tempPlayerTurns = new int[count];
        count = 0;
        for(int i = 0; i < playersData.Length;i++){
            if(playersData[i].playerStatus == TeenPatti.PlayerStatus.READY && playersData[i].isOccupied){
                tempPlayerTurns[count] = i;
                count++;
            }
        }   
        Debug.LogError("RESTART GAME 2");
        PhotonView.RPC("StartGame", RpcTarget.All, startIndex, tempPlayerTurns, tempCards, tempCardScores, jokerValue);
    }
    

    [PunRPC] public void StartGame(int startIndex, int[] playerTurns, int[] cards, double[] cardScores, int jokerValue){
        Debug.LogError("RESTART GAME 3");        
        //Debug.LogError("START GAME (RPC)");.
        roomData.jokerValue = jokerValue;
        roomData.roomStatus = TeenPatti.RoomStatus.INGAME;
        roomData.playersTurns = new int[playerTurns.Length];
        AssignCardAnimation(-1);
        TeenPattiGameManager.Instance.ActivateTimerObject(TablePosition[startIndex]);
        for (int i = 0; i < playerTurns.Length; i++){

            playersData[playerTurns[i]].cards  = new int[TPConstants.NUM_OF_CARDS];
            for (int j = 0; j <TPConstants.NUM_OF_CARDS; j++){
                int temp = i*TPConstants.NUM_OF_CARDS;
                playersData[playerTurns[i]].cards[j] = cards[temp+j];
                //TODO: This is to show cards // PlayerList[TablePosition[playerTurns[i]]].transform.Find("Cards").Find(j.ToString()).GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[playerTurns[i]].cards[j]]; //TODO:
                PlayerList[TablePosition[playerTurns[i]]].transform.Find("Cards").Find(j.ToString()).GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
            }
            for (int j = 0; j <TPConstants.NUM_OF_CARDS; j++){
                    PlayerList[TablePosition[playerTurns[i]]].transform.Find("VariationCards").Find(j.ToString()).GetComponentInChildren<Image>().color = new Color(1,1,1,1);
                    PlayerList[TablePosition[playerTurns[i]]].transform.Find("VariationCards").Find(j.ToString()).GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiScoreCalc.Instance.GetCardsOnVariation(playersData[playerTurns[i]].cards, jokerValue, (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE])[j]];
                    if(playersData[playerTurns[i]].cards == TeenPattiScoreCalc.Instance.GetCardsOnVariation(playersData[playerTurns[i]].cards, jokerValue, (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE])){
                        PlayerList[TablePosition[playerTurns[i]]].transform.Find("VariationCards").Find(j.ToString()).GetComponentInChildren<Image>().color = new Color(1,1,1,0);
                    }
            }

            playersData[playerTurns[i]].playPoint -= roomData.BootValue;
            PlayerList[TablePosition[playerTurns[i]]].transform.Find("PlayerTotalAmount").GetComponentInChildren<Text>().text =""+ playersData[playerTurns[i]].playPoint;
            playersData[playerTurns[i]].blindCount = TPConstants.BLIND_COUNT;
            playersData[playerTurns[i]].playerCardStatus = TeenPatti.PlayerCardStatus.BLIND;
            playersData[playerTurns[i]].cardScores = cardScores[i];
            roomData.playersTurns[i] = TablePosition[playerTurns[startIndex]];
            //Debug.LogError("POS "+TablePosition[playerTurns[startIndex]]);
            AssignCardAnimation(TablePosition[playerTurns[startIndex]]);

            PlayerList[roomData.playersTurns[i]].transform.Find("DummyCards").Find("CardStatus").Find("Status").GetComponentInChildren<Text>().text = "Blind";
            
            // timerText.text += " -  "+roomData.playersTurns[i];
            //Debug.LogError(roomData.playersTurns[i]);
            if(startIndex<playerTurns.Length-1){
                startIndex++;
            }
            else{
                startIndex = 0;
            }
            // PlayerList[TablePosition[i]].GetComponent<CanvasGroup>().alpha = 1f;
        }
        JokerObject.SetActive(false);
        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.JOKER){
            JokerObject.transform.Find("Image").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[jokerValue];
            JokerObject.SetActive(true);
        }
        Debug.LogError("RESTART GAME 4");        
        TeenPattiGameManager.Instance.CardAnimationPlaying();
        roomData.currentPlayerTurnIndex = 0;
        roomData.OnTableMoney = roomData.BootValue * playerTurns.Length;
        // chalIndex.text +=" / "+roomData.currentPlayerTurnIndex;

        PlayerList[TablePosition[playerTurns[startIndex]]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,0,1,1);
        
    }

    public void AssignCardAnimation(int i){
        if(i==1){
            TeenPattiGameManager.Instance.Player1Online = true;
        }
        if(i==2){
            TeenPattiGameManager.Instance.Player2Online = true;
        }
        if(i==3){
            TeenPattiGameManager.Instance.Player3Online = true;
        }
        if(i==4){
            TeenPattiGameManager.Instance.Player4Online = true;
        }
        if(i==-1){
            TeenPattiGameManager.Instance.Player1Online = false;
            TeenPattiGameManager.Instance.Player2Online = false;
            TeenPattiGameManager.Instance.Player3Online = false;
            TeenPattiGameManager.Instance.Player4Online = false;
        }    
    }
    public void RevokeCardAnimation(int i){
        Debug.Log("REVOKE" +i);
        if(i==1){
            TeenPattiGameManager.Instance.Player1Online = false;
        }
        if(i==2){
            TeenPattiGameManager.Instance.Player2Online = false;
        }
        if(i==3){
            TeenPattiGameManager.Instance.Player3Online = false;
        }
        if(i==4){
            TeenPattiGameManager.Instance.Player4Online = false;
        }
           
    }
    #endregion
    #region Action Panel Controller

    public void NextPlayer(){
        PhotonView.RPC("ActionNextPlayer", RpcTarget.All);
    }
    public void ShowCards(){
        PhotonView.RPC("ActionShowCards",RpcTarget.All, localPlayerIndex);
    }
    public void SideShowCards(int sideshow0, int sideshow1){
        PhotonView.RPC("ActionSideShowCards",RpcTarget.All, sideshow0, sideshow1, true, false);

    }
    public void PlayerPackCards(int localIndex){
        PhotonView.RPC("ActionPlayerPacks", RpcTarget.All, localIndex);
    }
    public void PlayerChaal(bool isPlus, int localIndex){
        PhotonView.RPC("ActionPlayerChaals", RpcTarget.All, isPlus, localIndex);

    }
    public void UpdatePlayerCoins(Player targetPlayer, string betType){
        int index = -1;
        for (int i = 0; i < playersData.Length; i++)
        {
            if(playersData[i].actorNumber == targetPlayer.ActorNumber){
                index = i;
                break;
            }
        }
        if(index != -1){
            if(betType == TPConstants.CHAAL || betType == TPConstants.SHOW || betType == TPConstants.SIDE_SHOW){
                UpdatePlayerCoinsAfterChaal(index, targetPlayer);
            }
            else if(betType == TPConstants.WIN){
                UpdatePlayerCoinsAfterWin(index, targetPlayer);
            }
            
        }
    }
    public void UpdatePlayerCoinsAfterChaal(int index, Player targetPlayer){
        float amount = roomData.currentChalValue;
        playersData[index].playPoint =  (float)targetPlayer.CustomProperties[TPConstants.PLAY_POINT];
        
        if(playersData[index].playerCardStatus == TeenPatti.PlayerCardStatus.SEEN){
            amount = roomData.currentChalValue*2;
        }
        PlayerList[TablePosition[index]].transform.Find("PlayerTotalAmount").GetComponentInChildren<Text>().text =""+ playersData[index].playPoint;
        TeenPattiGameManager.Instance.ChaalAnimation(TablePosition[index],amount, false);
        if(playersData[index].playerCardStatus == TeenPatti.PlayerCardStatus.BLIND){
            playersData[index].blindCount--;
            if(playersData[index].blindCount<=0 && PhotonNetwork.LocalPlayer.ActorNumber == playersData[index].actorNumber){
                PlayerSeenCards();
            }

        }
    }
    public void UpdatePlayerCoinsAfterWin(int index, Player targetPlayer){
        
        playersData[index].playPoint =  (float)targetPlayer.CustomProperties[TPConstants.PLAY_POINT];
        PlayerList[TablePosition[index]].transform.Find("PlayerTotalAmount").GetComponentInChildren<Text>().text =""+ playersData[index].playPoint;
        TeenPattiGameManager.Instance.ChaalAnimation(TablePosition[index],roomData.OnTableMoney, true);
        roomData.OnTableMoney = 0;

    }
    public void PlayerSeenCards(){
        TeenPattiGameManager.Instance.SeeBtn.SetActive(false);
        TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiTableManager.Instance.playersData[localPlayerIndex].cards[0]];
        TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiTableManager.Instance.playersData[localPlayerIndex].cards[1]];
        TeenPattiTableManager.Instance.PlayerList[0].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[TeenPattiTableManager.Instance.playersData[localPlayerIndex].cards[2]];
        PhotonView.RPC("ActionPlayerSeen", RpcTarget.All, localPlayerIndex);
    }
    
    [PunRPC] public void ActionPlayerLeaves(int localIndex){
        playersData[localIndex].playerStatus = TeenPatti.PlayerStatus.WAITING;
        // PlayerList[TablePosition[localIndex]].GetComponent<CanvasGroup>().alpha = 0.7f;
        PlayerList[TablePosition[localIndex]].transform.Find("Cards").Find("0").gameObject.SetActive(false);
        PlayerList[TablePosition[localIndex]].transform.Find("Cards").Find("1").gameObject.SetActive(false);
        PlayerList[TablePosition[localIndex]].transform.Find("Cards").Find("2").gameObject.SetActive(false);

        int startIndex = System.Array.IndexOf(roomData.playersTurns,TablePosition[localIndex]);
        roomData.playersTurns = roomData.playersTurns.Where((val, idx) => idx != startIndex).ToArray();
        if(roomData.playersTurns.Length==roomData.currentPlayerTurnIndex){
            roomData.currentPlayerTurnIndex--;
        }
        if(actionPanel.activeInHierarchy){
            TeenPattiActionPanel.Instance.UpdateActionPanelValues();
        }
        if(PhotonNetwork.IsMasterClient && roomData.playersTurns.Length <= 1){
            TeenPattiGameManager.Instance.ActivateTimerObjectOff();
            playerTakingTurn = false;
            if(roomData.playersTurns.Length == 1 && roomData.OnTableMoney>0){
                DecideWinner(TeenPatti.DecideWinner.ALL_PACK);
            }
        }  
    }
    [PunRPC] public void ActionPlayerPacks(int localIndex){
        if(localIndex == localPlayerIndex){
            if(TeenPattiGameManager.Instance.SeeBtn.activeInHierarchy){
                TeenPattiGameManager.Instance.SeeBtn.SetActive(false);
            }
            PlayerList[TablePosition[localIndex]].transform.Find("Cards").Find("0").gameObject.SetActive(false);
            PlayerList[TablePosition[localIndex]].transform.Find("Cards").Find("1").gameObject.SetActive(false);
            PlayerList[TablePosition[localIndex]].transform.Find("Cards").Find("2").gameObject.SetActive(false);
        }
        playersData[localIndex].playerStatus = TeenPatti.PlayerStatus.WAITING;
        // PlayerList[TablePosition[localIndex]].GetComponent<CanvasGroup>().alpha = 0.7f;
        PlayerList[TablePosition[localIndex]].transform.Find("DummyCards").Find("0").gameObject.SetActive(false);
        PlayerList[TablePosition[localIndex]].transform.Find("DummyCards").Find("1").gameObject.SetActive(false);
        PlayerList[TablePosition[localIndex]].transform.Find("DummyCards").Find("2").gameObject.SetActive(false);
        
        PlayerList[TablePosition[localIndex]].transform.Find("DummyCards").Find("CardStatus").gameObject.SetActive(false);
        
        
        //Debug.LogError("INDEX POS : "+System.Array.IndexOf(roomData.playersTurns,TablePosition[localIndex]));
        int startIndex = System.Array.IndexOf(roomData.playersTurns,TablePosition[localIndex]);
        roomData.playersTurns = roomData.playersTurns.Where((val, idx) => idx != startIndex).ToArray();

            for (int i = 0; i < startIndex; i++){
            var temp = roomData.playersTurns[0];
                for (var j = 0; j < roomData.playersTurns.Length - 1; j++)
                {
                    roomData.playersTurns[j] = roomData.playersTurns[j + 1];
                }
                roomData.playersTurns[roomData.playersTurns.Length - 1] = temp;
            }
            roomData.currentPlayerTurnIndex = 0;
        
        if(actionPanel.activeInHierarchy){
            TeenPattiActionPanel.Instance.UpdateActionPanelValues();
        }
        if(roomData.playersTurns.Length !=0){
            if(roomData.playersTurns[roomData.currentPlayerTurnIndex]==0){
                TeenPattiActionPanel.Instance.UpdateActionPanelValues();
                actionPanel.SetActive(true);
            }
            else{
                actionPanel.SetActive(false);
            }
        }
        ResetPlayerTurnTime();
    }
    [PunRPC] public void ActionPlayerChaals(bool isPlus,int localIndex){
        if(isPlus){
            roomData.currentChalValue *= 2;
        }
        float amount = roomData.currentChalValue;
        //Debug.LogError("ASDDDD" + localIndex);
        if(playersData[localIndex].playerCardStatus == TeenPatti.PlayerCardStatus.SEEN){
            //Debug.LogError("HE HAS SEEN ASDDDD" + localIndex);
            amount = roomData.currentChalValue*2;
        }
        roomData.OnTableMoney+=amount;

        // ResetPlayerTurnTime();
    }
    [PunRPC] public void ActionPlayerSeen(int localIndex){
        playersData[localIndex].playerCardStatus = TeenPatti.PlayerCardStatus.SEEN;
        if(actionPanel.activeInHierarchy){
            TeenPattiActionPanel.Instance.UpdateActionPanelValues();
        }
        PlayerList[TablePosition[localIndex]].transform.Find("DummyCards").Find("CardStatus").Find("Status").GetComponentInChildren<Text>().text = "Seen"; 

        // ResetPlayerTurnTime();
    }
    [PunRPC] public void ActionNextPlayer(){
        if(roomData.currentPlayerTurnIndex>=roomData.playersTurns.Length-1){
            roomData.currentPlayerTurnIndex=0;
        }
        else{
            roomData.currentPlayerTurnIndex++;
        }
        // chalIndex.text +=" / "+roomData.currentPlayerTurnIndex;
        PlayerList[TablePosition[0]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,1,0 ,1);
        PlayerList[TablePosition[1]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,1,0 ,1);
        PlayerList[TablePosition[2]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,1,0 ,1);
        PlayerList[TablePosition[3]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,1,0 ,1);
        PlayerList[TablePosition[4]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,1,0 ,1);
        PlayerList[roomData.playersTurns[roomData.currentPlayerTurnIndex]].transform.Find("PlayerStatus").GetComponentInChildren<Image>().color = new Color(0,0,1,1);
        if(roomData.playersTurns[roomData.currentPlayerTurnIndex]==0){
            TeenPattiActionPanel.Instance.UpdateActionPanelValues();
            actionPanel.SetActive(true);
        }
        else{
            actionPanel.SetActive(false);
        }
        ResetPlayerTurnTime();
        
    }
    
    
    public void ResetPlayerTurnTime(){
        roomData.playersTurnsTimer = TPConstants.PLAYER_TURN_WATING_TIME;
        playerTakingTurn = true;
        //Debug.LogError("CALLED "+roomData.playersTurns.Length);
        TeenPattiGameManager.Instance.ActivateTimerObjectOff();
        if(roomData.playersTurns.Length == 1 || roomData.OnTableMoney >= roomData.PotLimit){
            playerTakingTurn = false;
            if(PhotonNetwork.IsMasterClient){
                if(roomData.playersTurns.Length == 1 && roomData.OnTableMoney >0){
                    DecideWinner(TeenPatti.DecideWinner.ALL_PACK);
                }
                else if(roomData.OnTableMoney >= roomData.PotLimit){
                    DecideWinner(TeenPatti.DecideWinner.POT_LIMIT);
                }
            }
            //TODO: Declare Winner
            // RestartGame();
            //Debug.LogError("IN-RESTARTCALLED");
        }
        else{
            TeenPattiGameManager.Instance.ActivateTimerObject(roomData.playersTurns[roomData.currentPlayerTurnIndex]);
        }
        // else if(roomData.playersTurns.Length!=0){
        // }

    }
    
    [PunRPC] public void ActionSideShowCards(int sideShow0, int sideShow1, bool isAsk , bool isAccept){

        tempSideShow0 = -1;
        tempSideShow1 = -1;
        if(isAsk && !isAccept){
            if(actionPanel.activeInHierarchy){
            actionPanel.SetActive(false);
            }
            if(TablePosition[sideShow1]==0){
                tempSideShow0 = sideShow0;
                tempSideShow1 = sideShow1;
                TeenPattiGameManager.Instance.ShowSideShowPanel(playersData[sideShow0].name, playersData[sideShow0].avatarId, sideShow0, sideShow1);
            }
        }
        else if (!isAsk && isAccept){
            if(PhotonNetwork.IsMasterClient){
                DecideWinner(TeenPatti.DecideWinner.SIDE_SHOW, sideShow0:sideShow0, sideShow1: sideShow1);
            }
        }
        else if (!isAsk && !isAccept){
            if(TablePosition[sideShow0]==0){
                NextPlayer();
            }
        }
        
    }
    [PunRPC] public void ActionShowCards(int localIndex){
        playerTakingTurn = false;
        if(PhotonNetwork.IsMasterClient){
            DecideWinner(TeenPatti.DecideWinner.SHOW, showIndex:localIndex);
        }
    }
    public void DecideWinner(TeenPatti.DecideWinner decideWinner, int showIndex = -1, int sideShow0 =-1 , int sideShow1 = -1){
        int winnerIndexNumber = -1;
        int value = System.Array.IndexOf(TablePosition,roomData.playersTurns[0]);
        if(decideWinner == TeenPatti.DecideWinner.ALL_PACK){
            winnerIndexNumber = value;
        }

        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.MUFLIS){
            if(decideWinner == TeenPatti.DecideWinner.POT_LIMIT){
                winnerIndexNumber = value;
                for(int i = 0; i < roomData.playersTurns.Length; i++){
                    int insideValue = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
                    if(playersData[winnerIndexNumber].cardScores>playersData[insideValue].cardScores){
                        winnerIndexNumber = insideValue;
                    }
                }
            }
            if(decideWinner == TeenPatti.DecideWinner.SHOW){
                winnerIndexNumber = value;
                for(int i = 0; i < roomData.playersTurns.Length; i++){
                    int insideValue = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
                    if(playersData[winnerIndexNumber].cardScores>playersData[insideValue].cardScores){
                        winnerIndexNumber = insideValue;
                    }
                }
            }
            if(decideWinner == TeenPatti.DecideWinner.SIDE_SHOW){
                winnerIndexNumber = sideShow0;
                if(playersData[sideShow0].cardScores<playersData[sideShow1].cardScores){
                    winnerIndexNumber = sideShow1;
                }
            }
            
        }
        else{
            if(decideWinner == TeenPatti.DecideWinner.POT_LIMIT){
                winnerIndexNumber = value;
                for(int i = 0; i < roomData.playersTurns.Length; i++){
                    int insideValue = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
                    if(playersData[winnerIndexNumber].cardScores<playersData[insideValue].cardScores){
                        winnerIndexNumber = insideValue;
                    }
                }
            }
            if(decideWinner == TeenPatti.DecideWinner.SHOW){
                winnerIndexNumber = value;
                for(int i = 0; i < roomData.playersTurns.Length; i++){
                    int insideValue = System.Array.IndexOf(TablePosition,roomData.playersTurns[i]);
                    if(playersData[winnerIndexNumber].cardScores<playersData[insideValue].cardScores){
                        winnerIndexNumber = insideValue;
                    }
                }
            }
            if(decideWinner == TeenPatti.DecideWinner.SIDE_SHOW){
                winnerIndexNumber = sideShow0;
                if(playersData[sideShow0].cardScores>playersData[sideShow1].cardScores){
                    winnerIndexNumber = sideShow1;
                }
            }
        }

        if(winnerIndexNumber!=-1){
            PhotonView.RPC("ShowWinner", RpcTarget.All, winnerIndexNumber, (int)decideWinner, sideShow0, sideShow1);
        }
    }

    [PunRPC] public void ShowWinner(int winnerIndexNumber, int decideWinner, int sideShow0, int sideShow1){
        if(actionPanel.activeInHierarchy){
            actionPanel.SetActive(false);
        }
        
        if((TeenPatti.DecideWinner)decideWinner != TeenPatti.DecideWinner.SIDE_SHOW){
            timerText.text = "";
            // chalIndex.text = "";
            if(TeenPattiGameManager.Instance.SeeBtn.activeInHierarchy){
                TeenPattiGameManager.Instance.SeeBtn.SetActive(false);
            }
            for (int i = 0; i < roomData.playersTurns.Length; i++)
            {
                TeenPatti.RoomType roomType=  (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE];
                // if(roomType == TeenPatti.RoomType.AK47 || roomType == TeenPatti.RoomType.HIGHEST_JOKER || roomType == TeenPatti.RoomType.JOKER || roomType == TeenPatti.RoomType.LOWER_JOKER){
                PlayerList[roomData.playersTurns[i]].transform.Find("VariationCards").gameObject.SetActive(true);
                // }
                int startIndex = System.Array.IndexOf(TablePosition, roomData.playersTurns[i]);
                Debug.LogError(startIndex+" TBPOS "+roomData.playersTurns[i]);
                if((TeenPatti.DecideWinner)decideWinner != TeenPatti.DecideWinner.ALL_PACK){
                    PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[startIndex].cards[0]];
                    PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[startIndex].cards[1]];
                    PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[startIndex].cards[2]];
                    PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("0").gameObject.SetActive(true);
                    PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("1").gameObject.SetActive(true);
                    PlayerList[roomData.playersTurns[i]].transform.Find("Cards").Find("2").gameObject.SetActive(true);
                }
                }
            if(TablePosition[winnerIndexNumber] == 0){
                Debug.Log("ShowWinner Called");
                TeenPattiActionPanel.Instance.UpdateMoney(roomData.OnTableMoney, TPConstants.WIN);            
            }
            PhotonView.RPC("ActionWaitRestartGame",RpcTarget.All);
        }
        else{
            //TODO: IN SIDE SHOW WINNER INDEX IS ACTUALLY LOSER INDEX
            if(TablePosition[sideShow0]==0 || TablePosition[sideShow1]==0){
                StartCoroutine(SideShowCardsHide(sideShow0,sideShow1, winnerIndexNumber));
            }
        }
        if((TeenPatti.DecideWinner)decideWinner != TeenPatti.DecideWinner.SIDE_SHOW){
            PlayerList[TablePosition[winnerIndexNumber]].transform.Find("WinnerAnim").GetComponentInChildren<Animator>().Play("WinnerAnimation");
            PlayerList[TablePosition[winnerIndexNumber]].transform.Find("RoundAnim").GetComponentInChildren<Animator>().Play("RoundAnim");
        }
        else{
            SideShowAnimObject.transform.Find("SideShow0").Find("Avatar").GetComponentInChildren<Image>().sprite = MainDataManager.Instance.avatars[playersData[sideShow0].avatarId];
            SideShowAnimObject.transform.Find("SideShow1").Find("Avatar").GetComponentInChildren<Image>().sprite = MainDataManager.Instance.avatars[playersData[sideShow1].avatarId];
            SideShowAnimObject.transform.Find("SideShow0").Find("Name").GetComponentInChildren<Text>().text = playersData[sideShow0].name;
            SideShowAnimObject.transform.Find("SideShow1").Find("Name").GetComponentInChildren<Text>().text = playersData[sideShow1].name;
            if(winnerIndexNumber==sideShow0){
                SideShowAnimObject.transform.Find("SideShow1").Find("WinnerAnim").gameObject.SetActive(true);
                SideShowAnimObject.transform.Find("SideShow0").Find("WinnerAnim").gameObject.SetActive(false);

            }
            else{
                SideShowAnimObject.transform.Find("SideShow0").Find("WinnerAnim").gameObject.SetActive(true);
                SideShowAnimObject.transform.Find("SideShow1").Find("WinnerAnim").gameObject.SetActive(false);
            }
            SideShowAnimObject.GetComponent<Animator>().Play("SideShowAnimation");
        }
        
        
    }
    IEnumerator SideShowCardsHide(int sideShow0,int sideShow1, int winnerIndexNumber){
        if(TeenPattiGameManager.Instance.SeeBtn.activeInHierarchy){
            TeenPattiGameManager.Instance.SeeBtn.SetActive(false);
        }
        PlayerList[TablePosition[sideShow0]].transform.Find("VariationCards").gameObject.SetActive(true);       
        PlayerList[TablePosition[sideShow1]].transform.Find("VariationCards").gameObject.SetActive(true);
        PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[sideShow0].cards[0]];
        PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[sideShow0].cards[1]];
        PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[sideShow0].cards[2]];
        PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[sideShow1].cards[0]];
        PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[sideShow1].cards[1]];
        PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardImages[playersData[sideShow1].cards[2]];   
        PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("0").gameObject.SetActive(true);
        PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("1").gameObject.SetActive(true);
        PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("2").gameObject.SetActive(true);
        PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("0").gameObject.SetActive(true);
        PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("1").gameObject.SetActive(true);
        PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("2").gameObject.SetActive(true); 
        yield return new WaitForSeconds(2f);
        PlayerList[TablePosition[sideShow0]].transform.Find("VariationCards").gameObject.SetActive(false);        
        PlayerList[TablePosition[sideShow1]].transform.Find("VariationCards").gameObject.SetActive(false);
        if(TablePosition[sideShow0]==0){
            // PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
            // PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
            // PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
            PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("0").gameObject.SetActive(false);
            PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("1").gameObject.SetActive(false);
            PlayerList[TablePosition[sideShow1]].transform.Find("Cards").Find("2").gameObject.SetActive(false);    
        }
        if(TablePosition[sideShow1]==0){
            // PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("0").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
            // PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("1").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;
            // PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("2").GetComponentInChildren<Image>().sprite = TeenPattiScoreCalc.Instance.cardBg;    
            PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("0").gameObject.SetActive(false); 
            PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("1").gameObject.SetActive(false); 
            PlayerList[TablePosition[sideShow0]].transform.Find("Cards").Find("2").gameObject.SetActive(false);     
        }
        if(TablePosition[winnerIndexNumber] == 0){
                
            Debug.Log("SideShowWinner Called");
            TeenPattiActionPanel.Instance.PackButtonClick();            
            if(winnerIndexNumber == sideShow1){
                NextPlayer();
            }
        }
    }

    int tempSideShow0 = -1;
    int tempSideShow1 = -1;
    public void SideShowAccept(){
        if(tempSideShow0 != -1 && tempSideShow1 != -1){
            TeenPattiGameManager.Instance.SideShowPanel.SetActive(false);
            PhotonView.RPC("ActionSideShowCards",RpcTarget.All, tempSideShow0, tempSideShow1, false, true);
        }
    }
    public void SideShowDecline(){
        if(tempSideShow0 != -1 && tempSideShow1 != -1){
            TeenPattiGameManager.Instance.SideShowPanel.SetActive(false);
            PhotonView.RPC("ActionSideShowCards",RpcTarget.All, tempSideShow0, tempSideShow1, false, false);
        }
    }


    [PunRPC] public void ActionWaitRestartGame(){
        timerText.text = "Wait for next Round to start";
        playerTakingTurn = false;
        start = false;
        roomData.roomStatus = TeenPatti.RoomStatus.WAITING;
        if(MainDataManager.Instance.playPoint<roomData.BootValue){
            StartCoroutine(LeaveRoomBecauseofMoney());
        }
        Invoke("MasterHandleRestart", TPConstants.MATCH_RESTART_TIME);
    }
    IEnumerator LeaveRoomBecauseofMoney(){
        yield return new WaitForSeconds(TPConstants.MATCH_RESTART_TIME+3f);
        PhotonNetwork.LeaveRoom(false);
    }
    public void MasterHandleRestart(){
        if(PhotonNetwork.IsMasterClient){
            PhotonView.RPC("ActionRestartGame",RpcTarget.All);
        }
    }

    [PunRPC] public void ActionRestartGame(){
        timerText.text = "Wait for next Round to start";
        
        if((TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE] == TeenPatti.RoomType.FOURXBOOT){
            roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*4;
        }
        else{
            roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE];
        }
        
        JokerObject.SetActive(false);
        roomData.ChalLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*128;
        roomData.PotLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE]*1024;
        roomData.currentChalValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE];
        roomData.currentPlayerTurnIndex = 0;
        roomData.watingTime = TPConstants.PLAYER_WAITING_TIME;
        roomData.OnTableMoney = 0;
        roomData.roomStatus = TeenPatti.RoomStatus.WAITING;
        roomData.roomType = (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE];
        roomData.playersTurnsTimer = TPConstants.PLAYER_TURN_WATING_TIME;
        
        for (int i = 0; i < playersData.Length; i++)
        {
            for (int j = 0; j < playersData[i].cards.Length; j++)
            {
                playersData[i].cards[j] = -1;
            }
            playersData[i].cardScores = -1;
            playersData[i].blindCount = 4;
            playersData[i].playerCardStatus = TeenPatti.PlayerCardStatus.BLIND;
            playersData[i].playerStatus = TeenPatti.PlayerStatus.READY;
        }
        for (int i = 0; i < PlayerList.Length; i++)
        {
            PlayerList[i].transform.Find("VariationCards").gameObject.SetActive(false);
            PlayerList[i].transform.Find("Cards").Find("0").gameObject.SetActive(false);
            PlayerList[i].transform.Find("Cards").Find("1").gameObject.SetActive(false);
            PlayerList[i].transform.Find("Cards").Find("2").gameObject.SetActive(false);
            PlayerList[i].transform.Find("DummyCards").Find("0").gameObject.SetActive(false);
            PlayerList[i].transform.Find("DummyCards").Find("1").gameObject.SetActive(false);
            PlayerList[i].transform.Find("DummyCards").Find("2").gameObject.SetActive(false);
            PlayerList[i].transform.Find("DummyCards").Find("CardStatus").gameObject.SetActive(false);
            PlayerList[i].transform.Find("DummyCards").Find("CardStatus").Find("Status").GetComponentInChildren<Text>().text = "Blind";
            

        }

        Debug.LogError("RESTART GAME "+(int)roomData.roomType);

        playerTakingTurn = false;
        start = true;
    }


    // public void RestartGame(){
    //     //Debug.LogError("RESTARTCALLED");
    //     roomData.BootValue = (float)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.BOOT_VALUE];
    //     roomData.roomType = (TeenPatti.RoomType)PhotonNetwork.CurrentRoom.CustomProperties[TPConstants.VARIATION_TYPE];
    //     roomData.ChalLimit = roomData.BootValue*128;
    //     roomData.PotLimit = roomData.BootValue*1024;
    //     roomData.OnTableMoney = 0;
    //     roomData.roomStatus = TeenPatti.RoomStatus.WAITING;
    //     roomData.currentChalValue = roomData.BootValue;
    //     roomData.watingTime = TPConstants.PLAYER_WAITING_TIME;
    //     roomData.playersTurnsTimer = TPConstants.PLAYER_TURN_WATING_TIME;
    //     timerText.text = "Waiting For other players to Join !!";
    //     for(int i = 0; i < playersData.Length;i++){
    //         if(playersData[i].isOccupied){
    //             playersData[i].playerStatus = TeenPatti.PlayerStatus.READY;
    //         }
    //     }  
    //     //Debug.LogError("RESTARTCALLED "+roomData.watingTime);
    //     playerTakingTurn = false;
    //     start = true;
    // }
    

    #endregion
}
