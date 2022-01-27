using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonConnectionLogin : MonoBehaviourPunCallbacks
{
    

    public static PhotonConnectionLogin Instance;
    public GameObject WaitForRoom;
    public GameObject MasterDisconnected;


    bool isConnecting;
    void Awake(){
        PhotonNetwork.AutomaticallySyncScene = true;
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        WaitForRoom.SetActive(false);
        MasterDisconnected.SetActive(false);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // Connect();
    }
    void Connect() {
        
		isConnecting = true;

		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (!PhotonNetwork.IsConnected) 	
		{
            
            PhotonNetwork.NickName = MainDataManager.Instance.userName;
			Debug.Log("Connecting...");
			PhotonNetwork.GameVersion = "0.2";
			PhotonNetwork.ConnectUsingSettings();
		}
	}
    public void UpdatePlayerCoins(string betType){
        // Debug.LogError("00");
        Hashtable _updateProperties = new Hashtable();
        _updateProperties[TPConstants.PLAY_POINT] = MainDataManager.Instance.playPoint;
        _updateProperties[TPConstants.BET_TYPE] = betType;
        PhotonNetwork.LocalPlayer.SetCustomProperties(_updateProperties);
        // Debug.LogError("11");

    }
    IEnumerator ShowHidePanel(int time){
        NetworkManagerMain.Instance.errorText.text = "";
        PhotonConnectionLogin.Instance.WaitForRoom.SetActive(true);
        yield return new WaitForSeconds(time);
        PhotonConnectionLogin.Instance.WaitForRoom.SetActive(false);
        NetworkManagerMain.Instance.errorText.text = "Failed To Connect Please Try Again";


    }
    public void TeenPattiJoinRoom()
    {
        if(PhotonNetwork.IsConnected){
            StartCoroutine(ShowHidePanel(5));
            Hashtable _myCustomProperties = new Hashtable();
            _myCustomProperties.Add(TPConstants.PLAY_POINT, MainDataManager.Instance.playPoint);
            _myCustomProperties.Add(TPConstants.AVATAR_ID, MainDataManager.Instance.avatarId);
            _myCustomProperties.Add(TPConstants.PLAYER_NAME, MainDataManager.Instance.userName);
            PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;

            
            // if there are avilable rooms, join a random one
            if (PhotonNetwork.CountOfRooms > 0){

                Hashtable expectedCustomRoomProperties = new Hashtable();
                expectedCustomRoomProperties.Add(TPConstants.VARIATION_TYPE, (int)MainDataManager.Instance.teenPattiRoomType);
                expectedCustomRoomProperties.Add(TPConstants.BOOT_VALUE, MainDataManager.Instance.tableBootValues[SetScreenValues.Instance.currentPlay]);
                PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties,TPConstants.MAX_PLAYERS);
                
            }
            //otherwise, create a new one
            else
            {

                TeenPattiCreateRoom();
            }
        }
        else{
            NetworkManagerMain.Instance.errorText.text = "Please Wait We are not connected";
        }
            
    }
    public void TeenPattiCreateRoom(){
        string roomName = "TEENPATTI_"+Random.Range(0,999999).ToString("0000000");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = TPConstants.MAX_PLAYERS;
        options.IsOpen = true;
        options.IsVisible = true;
        options.PlayerTtl = 15000;
        options.CustomRoomProperties = new Hashtable();
        options.CustomRoomProperties.Add(TPConstants.VARIATION_TYPE, MainDataManager.Instance.teenPattiRoomType);
        options.CustomRoomProperties.Add(TPConstants.BOOT_VALUE, MainDataManager.Instance.tableBootValues[SetScreenValues.Instance.currentPlay]);
        options.CustomRoomPropertiesForLobby = new string[] {
			TPConstants.VARIATION_TYPE,
            TPConstants.BOOT_VALUE
		};
        PhotonNetwork.CreateRoom(roomName, options);
    }

    // public override void OnJoinRoomFailed(short returnCode, string message)
    // {
    //     Debug.LogError("NoROOm");
    //     TeenPattiCreateRoom();
    // }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError("NoROOm");
        TeenPattiCreateRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.LogError("NoROOm2");
        PhotonNetwork.LoadLevel(TPConstants.TEEN_PATTI_SCENE);
        Debug.LogError(PhotonNetwork.NickName+" joined in "+PhotonNetwork.CurrentRoom.Name);
        PlayerPrefs.SetString(TPConstants.PREVIOUS_ROOM,PhotonNetwork.CurrentRoom.Name);
        
    }
    public bool isSwitch;
    public override void OnLeftRoom()
    {
        if(isSwitch){
            TeenPattiJoinRoom();
        }
        else{
            SceneManager.LoadScene(TPConstants.MAIN_MENU_SCENE);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // if(newPlayer.CustomProperties.ContainsKey(PLAYER_NAME))
        //     Debug.Log(newPlayer.CustomProperties[PLAYER_NAME]);
        
        if(PhotonNetwork.IsMasterClient){
            if(newPlayer.HasRejoined){
                TeenPattiTableManager.Instance.RejoinedPlayer(newPlayer);
            }
            else{
                TeenPattiTableManager.Instance.AddPlayer(newPlayer);
            }
        }
        Debug.LogError(newPlayer.ActorNumber + " joined "+PhotonNetwork.CurrentRoom.Name);

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Is Player Inactive "+otherPlayer.IsInactive.ToString());
        if(otherPlayer.IsInactive){

        }
        else{
            RemovePlayerFromData(otherPlayer);
        }
            
    }


    public void RemovePlayerFromData(Player otherPlayer){
        int startIndex = -1;
            for (int i = 0; i < TeenPattiTableManager.Instance.playersData.Length; i++)
            {
                if(TeenPattiTableManager.Instance.playersData[i].actorNumber ==  otherPlayer.ActorNumber){
                    startIndex = i;
                    break;
                }
            }
            // Debug.LogError(startIndex+"  :: ::: " +TeenPattiTableManager.Instance.roomData.playersTurns[TeenPattiTableManager.Instance.roomData.currentPlayerTurnIndex]+"  :: ::: " +TeenPattiTableManager.Instance.TablePosition[startIndex]);
            if(PhotonNetwork.IsMasterClient){

                Debug.LogError("I AM MASTER");
                if(TeenPattiTableManager.Instance.roomData.playersTurns.Length !=0){
                    if(TeenPattiTableManager.Instance.roomData.playersTurns[TeenPattiTableManager.Instance.roomData.currentPlayerTurnIndex]==TeenPattiTableManager.Instance.TablePosition[startIndex]){
                        TeenPattiTableManager.Instance.PhotonView.RPC("ActionPlayerPacks", RpcTarget.All, startIndex);
                    }
                    else{
                        TeenPattiTableManager.Instance.PhotonView.RPC("ActionPlayerLeaves", RpcTarget.All, startIndex);
                    }
                }
                

                TeenPattiTableManager.Instance.RemovePlayer(otherPlayer);
            }
            Debug.LogError(otherPlayer.NickName + " left "+PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(changedProps.ContainsKey(TPConstants.PLAY_POINT)){
            if(changedProps.ContainsKey(TPConstants.BET_TYPE) && TPConstants.CHAAL == (string)changedProps[TPConstants.BET_TYPE]){
                TeenPattiTableManager.Instance.UpdatePlayerCoins(targetPlayer, TPConstants.CHAAL);
            }
            if(changedProps.ContainsKey(TPConstants.BET_TYPE) && TPConstants.WIN == (string)changedProps[TPConstants.BET_TYPE]){
                TeenPattiTableManager.Instance.UpdatePlayerCoins(targetPlayer, TPConstants.WIN);
            }
            if(changedProps.ContainsKey(TPConstants.BET_TYPE) && TPConstants.SHOW == (string)changedProps[TPConstants.BET_TYPE]){
                TeenPattiTableManager.Instance.UpdatePlayerCoins(targetPlayer, TPConstants.SHOW);
            }
            if(changedProps.ContainsKey(TPConstants.BET_TYPE) && TPConstants.SIDE_SHOW == (string)changedProps[TPConstants.BET_TYPE]){
                TeenPattiTableManager.Instance.UpdatePlayerCoins(targetPlayer, TPConstants.SIDE_SHOW);
            }

        }
        
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("DISCONNECTED CAUSE " +cause);
        
        // if(PhotonNetwork.InRoom){
        //     PhotonNetwork.LeaveRoom(true);
        // }
        MasterDisconnected.SetActive(true);

        if(!PhotonNetwork.ReconnectAndRejoin()){
            Connect();
        }
        // Connect();
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            MasterDisconnected.SetActive(false);
            NetworkManagerMain.Instance.LoginAgain();
            Debug.Log("PUN Launcher: OnConnectedToMaster() was called by PUN.");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.RejoinRoom(PlayerPrefs.GetString(TPConstants.PREVIOUS_ROOM,""));
        }
    }
    
    


// Update is called once per frame
    void Update()
    {
        // Debug.Log(PhotonNetwork.IsConnected);
        if(!PhotonNetwork.IsConnected){
            Debug.Log("DICSDSD");
            MasterDisconnected.SetActive(true);
            if(!PhotonNetwork.ReconnectAndRejoin()){
                Connect();
            }
        }
        if(PhotonNetwork.IsConnected && MasterDisconnected.activeInHierarchy){
            MasterDisconnected.SetActive(false);
        }
    }
}
