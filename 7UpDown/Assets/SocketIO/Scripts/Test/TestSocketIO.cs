#region License
/*
 * TestSocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 Fabio Panettieri
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System.Collections;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSocketIO : MonoBehaviour
{
    //amazon =   http://192.168.0.104:8000/api/auth   13.232.215.239:5000

    public static string myIp = "http://13.232.68.239:5001";
    private SocketIOComponent socket;
    public static TestSocketIO Instance;
    JSONObject data;
    bool notwait = true;

    public GameObject RejoinObject;

    public bool isOpen = false;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public string getMyIp
    {
        get { return myIp; }
        set
        {
            myIp = value;
        }
    }

    public void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("open", TestOpen);

        socket.On("res", GetData);
        socket.On("USER_REGISTER", UserRegister);
        socket.On("USER_LOGIN", UserLogin);
        socket.On("GET_BET_LISTS", GetBetLists);
        socket.On("hb", OnReceivePong);

        socket.On("boop", TestBoop);

        socket.On("error", TestError);
        socket.On("close", TestClose);
        StartCoroutine(CheckPingTimeOut());
        // Invoke("SendBeep", 5f);



    }
    public void OnReceivePong(SocketIOEvent e)
    {
        //Debug.Log("HB Receded >>>>");
    }

    public void UserRegister(SocketIOEvent e)
    {

    }

    public void UserLogin(SocketIOEvent e)
    {

    }
    public void GetBetLists(SocketIOEvent e)
    {

    }
    IEnumerator CheckPingTimeOut()
    {
        SB:
        yield return new WaitForSecondsRealtime(5);
        socket.Emit("hb", new JSONObject());
        goto SB;
    }



    public void SendBeep()
    {
        // StartCoroutine("BeepBoop");
    }
    private IEnumerator BeepBoop()
    {
        while (true)
        {
            // Debug.Log("00000000000000000Beep");
            socket.Emit("beep");
            Invoke("NoInternet", 6f);
            yield return new WaitForSeconds(5);
        }
    }

    public void TestOpen(SocketIOEvent e)
    {
        isOpen = true;
        Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
        Debug.Log("Socket Open Call");
        Debug.Log("00000000000000000000000000000000000000000000000000000000000000000000000000000000");
    }

    public void TestBoop(SocketIOEvent e)
    {
        // Debug.Log("00000000000000000Boop");
        CancelInvoke("NoInternet");
        // NetworkManager.Instance.YesInter();
    }
    public void NoInternet()
    {
        // NetworkManager.Instance.NoInter();
    }
    public Text ErrorText;
    public GameObject LoadingPanel;
    public int CurrentPlaceBetData = 0;
    public void GetData(SocketIOEvent e)
    {


        ////GameManager.Instance.playerData.Clear();

        string en = e.data.GetField("en").ToString().Trim(new char[] { '"' });
        string msg = e.data.GetField("msg").ToString().Trim(new char[] { '"' });
        string err = e.data.GetField("err").ToString().Trim(new char[] { '"' });
        //print(err);

        // Debug.Log("Event Recieved : ========"+en);
        print(en + " <== RESPONSE RECIEVED DATA ==> " + e.data);




        ////print("i11111111111111::::::::::::::::::" + e.data.Count + "En is ::::::::::::" + en);
        if (e.data.HasField("data"))
        {
            //Debug.Log("DATA THERE");
            data = new JSONObject(e.data.GetField("data").ToString().Trim(new char[] { '"' }));
            //Debug.Log(data);    
        }


        if(en == "USER_REGISTER")
        {
            Debug.Log("USER_REGISTER");
        }

        if (en == "USER_LOGIN")
        {
            Debug.Log("USER_LOGIN");
            

            if (err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
            else
            {
                PlayerPrefs.SetInt("isLogin", 1);
                Debug.Log(data.ToString());



                RegisterData register = JsonUtility.FromJson<RegisterData>(data.ToString());

                //DecryptToken(register.token);
                MainDataManager.Instance.userName = register.display_user_name;
                MainDataManager.Instance.id = register.id;
                Debug.Log("fffg");
                Debug.Log(register._id);
                MainDataManager.Instance._id = register._id;
                MainDataManager.Instance.referralId = register.referralId;
                MainDataManager.Instance.uniqueId = register.unique_id;
                MainDataManager.Instance.mobile = register.mobile;
                MainDataManager.Instance.playPoint = register.chips;
                MainDataManager.Instance.safePoint = register.safePoint;
                MainDataManager.Instance.avatarId = register.avatarId;
                NetworkManagerMain.Instance.playerName.text = MainDataManager.Instance.userName;
                NetworkManagerMain.Instance.playerPlayPoints.text = MainDataManager.Instance.playPoint + "";
                NetworkManagerMain.Instance.playerId.text = "ID : " + MainDataManager.Instance.uniqueId;
                NetworkManagerMain.Instance.playerAvatar.sprite = MainDataManager.Instance.avatars[MainDataManager.Instance.avatarId];
                NetworkManagerMain.Instance.ActivatePanelMainLogin(NetworkManagerMain.Instance.AppContainerScreenPanel.name);
            }
            if (data.GetField("rejoin").ToString()=="true")
            {
                Debug.Log("Rejoin True");
                RejoinObject.SetActive(true);
                MainDataManager.Instance.lastGameID = data.GetField("table_id").ToString().Trim(new char[] { '"' });
            }
            else
            {
                Debug.Log("Rejoin False");
                RejoinObject.SetActive(false);
            }

        }

        if(en == "LUDO_TABLE_LEAVE")
        {
            SceneManager.LoadScene(0);
            NetworkManagerMain.Instance.errorText.text = msg;
        }

        if (en == "GET_BET_LISTS")
        {
            Debug.Log("GET_BET_LISTS");

            if (err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
            else
            {

                LudoMenuManager.Instance.DisplayAllLudoGames(data);
            }

        }

        if(en == "LUDO_FIND_TABLE")
        {
            if(err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
        }

        if(en == "LUDO_DONE_MATCH_MECKING")
        {
            /*LUDO_DONE_MATCH_MECKING <== RESPONSE RECIEVED DATA ==> { "err":false,"msg":"","errcode":"0000","en":"LUDO_DONE_MATCH_MECKING","data":{ "max_seat":4,"player_info":[{ "seat_index":0,"user_info":{ "_id":"6193c332d50a776027b6c0a7","user_name":"ludo_15","profile_url":"image_avatar/avatar_2.png","chips":750,"sck_id":"-VWXLJvzS1eV8H0oAAAF"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"},{ "seat_index":1,"user_info":{ "_id":"618a8cc1bef3162cdf20a403","user_name":"ludo_13","profile_url":"image_avatar/avatar_3.png","chips":250,"sck_id":"5KTuYN6LT0HVNFIMAAAI"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"},{ "seat_index":2,"user_info":{ "_id":"618a8cc1bef3162cdf20a403","user_name":"ludo_13","profile_url":"image_avatar/avatar_3.png","chips":250,"sck_id":"5KTuYN6LT0HVNFIMAAAI"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"},{ "seat_index":3,"user_info":{ "_id":"618a8cc1bef3162cdf20a403","user_name":"ludo_13","profile_url":"image_avatar/avatar_3.png","chips":250,"sck_id":"5KTuYN6LT0HVNFIMAAAI"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"}],"game_id":"69"},"title":"Alert","sendTime":"2021-11-18T06:42:53.374Z"}*/
            LudoDataManager.Instance.SetPlayersData(data,"MATCH");

        }
        if (en == "LUDO_REJOIN_GAME")
        {
            SceneManager.LoadScene("LudoGameScene");
            /*LUDO_DONE_MATCH_MECKING <== RESPONSE RECIEVED DATA ==> { "err":false,"msg":"","errcode":"0000","en":"LUDO_DONE_MATCH_MECKING","data":{ "max_seat":4,"player_info":[{ "seat_index":0,"user_info":{ "_id":"6193c332d50a776027b6c0a7","user_name":"ludo_15","profile_url":"image_avatar/avatar_2.png","chips":750,"sck_id":"-VWXLJvzS1eV8H0oAAAF"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"},{ "seat_index":1,"user_info":{ "_id":"618a8cc1bef3162cdf20a403","user_name":"ludo_13","profile_url":"image_avatar/avatar_3.png","chips":250,"sck_id":"5KTuYN6LT0HVNFIMAAAI"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"},{ "seat_index":2,"user_info":{ "_id":"618a8cc1bef3162cdf20a403","user_name":"ludo_13","profile_url":"image_avatar/avatar_3.png","chips":250,"sck_id":"5KTuYN6LT0HVNFIMAAAI"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"},{ "seat_index":3,"user_info":{ "_id":"618a8cc1bef3162cdf20a403","user_name":"ludo_13","profile_url":"image_avatar/avatar_3.png","chips":250,"sck_id":"5KTuYN6LT0HVNFIMAAAI"},"color":"blue","status":"play","is_robot":0,"dice_slot":[-1,-1,-1,-1],"dice_value":0,"new_game":4,"not_move_kukari_count":0,"play_dice_slot":[],"six_counter":0,"six_not_come_count":0,"turn_counter":0,"turn_miss":0,"win_dice_slot":[],"winner_index":0,"winner_status":"Loss"}],"game_id":"69"},"title":"Alert","sendTime":"2021-11-18T06:42:53.374Z"}*/
            LudoDataManager.Instance.SetPlayersData(data,"REJOIN");

        }

        if (en == "LUDO_TABLE_INFO")
        {
            Debug.Log("LUDO_TABLE_INFO");

            if (err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
            else
            {
                LudoMenuManager.Instance.StartMatchMaking(data);
            }
        }

        if(en == "LUDO_JOIN_TABLE")
        {
            Debug.Log("LUDO_JOIN_TABLE");

            if (err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
            else
            {
                LudoMenuManager.Instance.JoinedMatchMaking(data);
            }
        }
        if(en == "UPDATED_WALLET")
        {
            /*UPDATED_WALLET <== RESPONSE RECIEVED DATA ==> 
         * {"err":false,"msg":"","errcode":"0000","en":"UPDATED_WALLET","data":{"game_winning":0,"chips":740,"t":"Ludo Boot Collect"},"title":"Alert","send":"2021-11-18T06:42:51.323Z"}*/
            if(err == "true")
            {

            }
            else
            {
                MainDataManager.Instance.playPoint = float.Parse(data.GetField("chips").ToString().Trim(new char[] { '"' }));
            }

        }

        if (en == "LUDO_BET_COLLECT")
        {
            /*LUDO_BET_COLLECT <== RESPONSE RECIEVED DATA ==> { "err":false,"msg":"","errcode":"0000","en":"LUDO_BET_COLLECT","data":{ "bet":10,"game_id":69},"title":"Alert","sendTime":"2021-11-18T06:42:51.358Z"}*/
            if (err == "true")
            {

            }
            else
            {
                LudoMenuManager.Instance.BetCollectAnimation(data);
            }

        }
        if(en == "LUDO_USER_TURN_START")
        {
            LudoNetworkManager.Instance.SetPlayerTurn(data);
        }
        if (en == "LUDO_USER_TURN_MISS")
        {
            LudoNetworkManager.Instance.PlayerTurnMiss(data);
        }
        if (en == "LUDO_ROTATE_DICE")
        {
            //Debug.Log("sdfsd");
            LudoNetworkManager.Instance.PlayerRollDice(data);
        }
        if(en == "LUDO_MOVE_KUKARI")
        {   if (err == "true")
            {

            }
            else
            {
                LudoNetworkManager.Instance.LudoKukariMove(data);
            }
        }

        if(en == "LUDO_TOUR_LIST")
        {
            if (err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
            else
            {

                LudoMenuManager.Instance.DisplayAllTournamentGames(data);
            }
        }
        if(en == "LUDO_TOUR_JOIN")
        {
            if (err == "true")
            {
                NetworkManagerMain.Instance.errorText.text = msg;
            }
            else
            {
                NetworkManagerMain.Instance.errorText.text = msg;
                LudoMenuManager.Instance.CloseTournament();
            }
        }



























        if (en == "join")
        {
            print("Join data===" + data.ToString());
            if (data.GetField("gameName").ToString().Replace("\"", "") == "sevenUp")
            {
                SevenUpDownManager.Instance.GetData(data);
            }
        }
        if (en == "joinUser")
        {
            print("New join data===" + data.ToString());
            if (data.GetField("gameName").ToString().Replace("\"", "") == "sevenUp")
            {
                Debug.Log("ererererere");
                SevenUpDownManager.Instance.UpdatePlayersDataOnJoin(data);
            }
        }
        if (en == "result")
        {
            print("result data===" + data.ToString());
            if (data.GetField("gameName").ToString().Replace("\"", "") == "sevenUp")
            {
                SevenUpDownManager.Instance.GetNumber1 = int.Parse(data.GetField("paso1").ToString());
                SevenUpDownManager.Instance.GetNumber2 = int.Parse(data.GetField("paso2").ToString());
                SevenUpDownManager.Instance.WinData = data;
                for (int i = 0; i < SevenUpDownManager.Instance.datas.Length; i++)
                {
                    SevenUpDownManager.Instance.datas[i] = null;
                }
                CurrentPlaceBetData = 0;
            }
        }
        if (en == "leaveRoom")
        {
            print("Leave User===" + data.ToString());
            if (data.GetField("gameName").ToString().Replace("\"", "") == "sevenUp")
            {
                Debug.Log("ererererere");
                SevenUpDownManager.Instance.UpdatePlayersDataOnLeave(data);
            }
        } 
        if (en == "place7UpBet")
        {
            print("Leave User===" + data.ToString());
            if (data.GetField("gameName").ToString().Replace("\"", "") == "sevenUp")
            {
                // if(CurrentPlaceBetData>90){
                //     CurrentPlaceBetData = 0;
                // }
                CurrentPlaceBetData++;
                SevenUpDownManager.Instance.PlaceBetNumberCount++;
                SevenUpDownManager.Instance.datas[CurrentPlaceBetData] = data;
            }
        }
        if(en == "balanceUpdate"){
            float value = float.Parse(data.GetField("amount").ToString().Replace("\"", ""));
            string betType = data.GetField("betType").ToString().Replace("\"", "");
            // Debug.LogError(value);
            MainDataManager.Instance.playPoint = value;
            if(betType == TPConstants.CHAAL){
                PhotonConnectionLogin.Instance.UpdatePlayerCoins(TPConstants.CHAAL);
            }
            if(betType == TPConstants.WIN){
                PhotonConnectionLogin.Instance.UpdatePlayerCoins(TPConstants.WIN);
            }
            if(betType == TPConstants.SHOW){
                PhotonConnectionLogin.Instance.UpdatePlayerCoins(TPConstants.SHOW);
            }
            if(betType == TPConstants.SIDE_SHOW){
                PhotonConnectionLogin.Instance.UpdatePlayerCoins(TPConstants.SIDE_SHOW);
            }
        }
    }
    public void SendData(string eventName, JSONObject obj)
    {

        print("SOCKET EVENT SENT ==> " + eventName + " WITH OBJECT ==> " + obj);
        socket.Emit(eventName, obj);
    }

    public void SendDataWithoutObject(string eventName)
    {
        print("SOCKET EVENT SENT ==> " + eventName);
        socket.Emit(eventName);
    }
    public void TestError(SocketIOEvent e)
    {
        isOpen = false;
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    public void TestClose(SocketIOEvent e)
    {
        isOpen = false;
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }

    void ApplicationQuit()
    {
        /*Request:
        {
            "en" : "LUDO_TABLE_LEAVE",
        "data" :{
            }
        }*/
        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_TABLE_LEAVE");
        eventSend.AddField("data", "");

        TestSocketIO.Instance.SendData("req", eventSend);

    }

    public void RejoinEventSend()
    {
        /*Request:
        {
            "en" : "LUDO_REJOIN_GAME",
            "data" : {
                    "table_id" : "" // table_id from USER_LOGIN event response
        }
        }*/

        JSONObject createData = new JSONObject();
        createData.AddField("table_id", MainDataManager.Instance.lastGameID);
        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_REJOIN_GAME");
        eventSend.AddField("data", createData);

        TestSocketIO.Instance.SendData("req", eventSend);
    }
    public void LeaveGame()
    {
        /*Request:
         {
             "en" : "LUDO_TABLE_LEAVE",
         "data" :{
             }
         }*/
        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_TABLE_LEAVE");
        eventSend.AddField("data", "");

        TestSocketIO.Instance.SendData("req", eventSend);
    }
    void OnApplicationFocus(bool focus)
    {
        if (focus && SceneManager.GetActiveScene().name == "LudoGameScene") RejoinEventSend();
    }
}
