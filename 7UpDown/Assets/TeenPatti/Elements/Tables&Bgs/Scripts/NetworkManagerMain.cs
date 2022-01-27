using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.IO;



public class NetworkManagerMain : MonoBehaviour
{

    public static NetworkManagerMain Instance;
    string WebServerUrl = "http://13.232.68.239:5000/";

    void Start()
    {
        Instance = this;
        if(PlayerPrefs.GetInt("isLogin", 0) == 1){
            
            StartCoroutine(StartLogin(MainDataManager.Instance.login_user_name,MainDataManager.Instance.password));
            // ActivatePanelMainLogin(AppContainerScreenPanel.name);
        }
        else{
            ActivatePanelMainLogin(MainLogoScreenPanel.name);
        }

        PlayerPrefs.SetString("BASE_URL", WebServerUrl);
        StartCoroutine(GetSocket());
    }

    IEnumerator GetSocket()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(PlayerPrefs.GetString("BASE_URL") + "chooseServer?det=android"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                JSONObject ip_Urls = new JSONObject(www.downloadHandler.text);

                PlayerPrefs.SetString("BASE_URL", ip_Urls.GetField("config").GetField("BASE_URL").ToString());
                PlayerPrefs.SetString("SOCKET_URL", ip_Urls.GetField("SOCKET_URL").ToString());
                Debug.Log("SOCKET URL SET");
            }
        }
    }
    public void LoginAgain(){
        if(PlayerPrefs.GetInt("isLogin", 0) == 1){

            StartCoroutine(StartLogin(MainDataManager.Instance.login_user_name,MainDataManager.Instance.password));
            // ActivatePanelMainLogin(AppContainerScreenPanel.name);
        }
        else{
            ActivatePanelMainLogin(MainLogoScreenPanel.name);
        }
    }

   void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveGameClick();
        }
    }


    #region  LoginPage

    [Header("==> Main Scene Variables")]
    public Text errorText;
    [Header("__Panel GameObjects")]
    public GameObject MainLogoScreenPanel;
    public GameObject LoginScreenPanel;
    public GameObject RegisterScreenPanel;
    public GameObject ForgotPasswordScreenPanel;
    public GameObject AppContainerScreenPanel;
    public GameObject SelectBetCoinsPanel;

    [Header("__Login Variables")]
    public InputField loginMobNumText;
    public InputField loginPasswordText;

    [Header("__Register Variables")]
    public InputField registerMobNumText;
    public InputField registerOTPText;
    public InputField registerUsernameText;
    public InputField registerReferralIdText;
    public InputField registerPasswordText;
    public InputField registerPasswordConfirmText;

    [Header("__Forgot Password Variables")]
    public InputField forgotMobNumText;
    public InputField forgotOTPText;
    public InputField forgotPasswordText;
    public InputField forgotPasswordConfirmText;

    [Header("__App Container Variables")]
    public Text playerName;
    public Text playerId;
    public Text playerPlayPoints;
    public Image playerAvatar;




    //UTILITY FUNTIONS
    void DecryptToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length > 2)
        {
            var decode = parts[1];
            var padLength = 4 - decode.Length % 4;
            if (padLength < 4)
            {
                decode += new string('=', padLength);
            }
            var bytes = System.Convert.FromBase64String(decode);
            var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
            Debug.Log(userInfo);
            RegisterData register = JsonUtility.FromJson<RegisterData>(userInfo);
            MainDataManager.Instance.userName = register.display_user_name;
            MainDataManager.Instance.id = register.id;
            
            Debug.Log(register._id);
            MainDataManager.Instance.referralId = register.referralId;
            MainDataManager.Instance.uniqueId = register.unique_id;
            MainDataManager.Instance.mobile = register.mobile;
            MainDataManager.Instance.playPoint = register.chips;
            MainDataManager.Instance.safePoint = register.safePoint;
            MainDataManager.Instance.avatarId = register.avatarId;
            MainDataManager.Instance.profile_url = register.profile_url;
            MainDataManager.Instance.table_id = register.table_id;
            MainDataManager.Instance.game_winning = register.game_winning;
            playerName.text = MainDataManager.Instance.userName;
            playerPlayPoints.text = MainDataManager.Instance.playPoint+"";
            playerId.text = "ID : "+MainDataManager.Instance.uniqueId;
            playerAvatar.sprite = MainDataManager.Instance.avatars[MainDataManager.Instance.avatarId];
            ActivatePanelMainLogin(AppContainerScreenPanel.name);
 

            //TODO: WhatToDoAfterLogin

        }
    }
    public void ActivatePanelMainLogin(string panelNametoAct)
    {
        MainLogoScreenPanel.SetActive(panelNametoAct.Equals(MainLogoScreenPanel.name));
        LoginScreenPanel.SetActive(panelNametoAct.Equals(LoginScreenPanel.name));
        RegisterScreenPanel.SetActive(panelNametoAct.Equals(RegisterScreenPanel.name));
        ForgotPasswordScreenPanel.SetActive(panelNametoAct.Equals(ForgotPasswordScreenPanel.name));
        AppContainerScreenPanel.SetActive(panelNametoAct.Equals(AppContainerScreenPanel.name));
    }
    public void Logout(){
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainLogin");
    }

    public void LogOutClick()
    {
        NoteManager.Instance.HeaderText.text = "Confirm Logout";
        NoteManager.Instance.NoteText.text = " Do you want to logout the game? ";
        NoteManager.Instance.OKButton.onClick.AddListener(Logout);
        NoteManager.Instance.NotePanel.SetActive(true);
    }
    public void Start7Up(){
        errorText.text = "";
        if(MainDataManager.Instance.playPoint>=10){
            JSONObject joinUser = new JSONObject();
            joinUser.AddField("token", "Bearer " + MainDataManager.Instance.token);
            joinUser.AddField("gameName", "sevenUp");
            TestSocketIO.Instance.SendData("join", joinUser);
            SceneManager.LoadScene("SevenUpDown");
        }
        else{
            errorText.text = "Not Enough Rs. !! Need Minimum 10 Rs.";
        }
    }

    public void StartLudo()
    {
        PlayerPrefs.SetString("LudoSubType", "classic");

        errorText.text = "";
        if (MainDataManager.Instance.playPoint >= 10)
        {
            JSONObject getBetLists = new JSONObject();
            getBetLists.AddField("game_type", "online");
            getBetLists.AddField("sub_type", PlayerPrefs.GetString("LudoSubType"));

            JSONObject eventSend = new JSONObject();
            eventSend.AddField("en", "GET_BET_LISTS");
            eventSend.AddField("data", getBetLists);

            TestSocketIO.Instance.SendData("req", eventSend);
        }
        else
        {
            errorText.text = "Not Enough Rs. !! Need Minimum 10 Rs.";
        }
    }
    public void StartLudoTournament()
    {
        PlayerPrefs.SetString("LudoSubType", "tournament");

        errorText.text = "";
        if (MainDataManager.Instance.playPoint >= 10)
        {
            
            JSONObject eventSend = new JSONObject();
            eventSend.AddField("en", "LUDO_TOUR_LIST");
            eventSend.AddField("data", "");

            TestSocketIO.Instance.SendData("req", eventSend);
        }
        else
        {
            errorText.text = "Not Enough Rs. !! Need Minimum 10 Rs.";
        }
    }
    public void StartLudoQuick()
    {
        PlayerPrefs.SetString("LudoSubType", "quick");

        errorText.text = "";
        if (MainDataManager.Instance.playPoint >= 10)
        {
            JSONObject getBetLists = new JSONObject();
            getBetLists.AddField("game_type", "online");
            getBetLists.AddField("sub_type", PlayerPrefs.GetString("LudoSubType"));

            JSONObject eventSend = new JSONObject();
            eventSend.AddField("en", "GET_BET_LISTS");
            eventSend.AddField("data", getBetLists);

            TestSocketIO.Instance.SendData("req", eventSend);
        }
        else
        {
            errorText.text = "Not Enough Rs. !! Need Minimum 10 Rs.";
        }
    }

    public void StartTeenPattiNormal(string GameName){
        if(MainDataManager.Instance.playPoint<MainDataManager.Instance.tableBootValues[0]*10){
            errorText.text = "Not Enough Rs. !! Need Minimum 10 Rs.";
            return;
        }
        if(GameName == TPConstants.NO_VARIATION){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.NO_VARIATION;
        }
        else if(GameName == TPConstants.AK47){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.AK47;
        }
        else if(GameName == TPConstants.JOKER){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.JOKER;
        }
        else if(GameName == TPConstants.LOWER_JOKER){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.LOWER_JOKER;
        }
        else if(GameName == TPConstants.HIGHEST_JOKER){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.HIGHEST_JOKER;
        }
        else if(GameName == TPConstants.FOURXBOOT){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.FOURXBOOT;
        }
        else if(GameName == TPConstants.MUFLIS){
            MainDataManager.Instance.teenPattiRoomType =  TeenPatti.RoomType.MUFLIS;
        }
        SelectBetCoinsPanel.SetActive(true);
        SetScreenValues.Instance.SetTeenPattiMenu();
        
    }


    //BUTTON CLICK FUNTIONS
    public void OnLoginButtonPRessed()
    {
        errorText.text = "";
        if (loginMobNumText.text.Length <= 0)
        {
            errorText.text = "Mobile Number Can't Be Empty.";
            return;
        }
        if (loginPasswordText.text.Length <= 0)
        {
            errorText.text = "Password Can't Be Empty.";
            return;
        }
        StartCoroutine(StartLogin(loginMobNumText.text, loginPasswordText.text));
    }


    IEnumerator StartLogin(string mobileNum, string password)
    {
        SB:
        if (TestSocketIO.Instance.isOpen)
        {


            WWWForm form = new WWWForm();
            //form.AddField("mobile", mobileNum);
            //form.AddField("password", password);
            //form.AddField("versionCode", Application.version);
            /*Request:
                {
                    "en" : "USER_LOGIN",
                    "data" : {
                        "user_name": "ashvin",
                        "password" : "ashvin"
                }
                }*/


            JSONObject loginUser = new JSONObject();
            loginUser.AddField("user_name", mobileNum);
            loginUser.AddField("password", password);

            JSONObject eventSend = new JSONObject();
            eventSend.AddField("en", "USER_LOGIN");
            eventSend.AddField("data", loginUser);

            TestSocketIO.Instance.SendData("req", eventSend);
            MainDataManager.Instance.login_user_name = mobileNum;
            MainDataManager.Instance.password = password;
            yield return new WaitForSeconds(1);
        }
        else
        {
            goto SB;
        }
    }



    public void OnRegisterButtonPressed()
    {
        errorText.text = "";
        if (registerMobNumText.text.Length <= 0)
        {
            errorText.text = "Mobile Number Can't Be Empty.";
            return;
        }
        if (registerOTPText.text.Length <= 0)
        {
            errorText.text = "OTP Can't Be Empty.";
            return;
        }
        if (registerUsernameText.text.Length <= 0)
        {
            errorText.text = "Username Can't Be Empty.";
            return;
        }
        if (registerPasswordText.text.Length <= 0)
        {
            errorText.text = "Password Can't Be Empty.";
            return;
        }
        if (registerPasswordConfirmText.text.Length <= 0)
        {
            errorText.text = "Confirm Password Can't Be Empty.";
            return;
        }
        if (registerPasswordConfirmText.text != registerPasswordText.text)
        {
            errorText.text = "Password and Confirm Password Cannot Be Different";
            return;
        }
        StartCoroutine(StartRegister());
    }
    IEnumerator StartRegister()
    {
        /*Request:
            {
                "en" : "USER_REGISTER",
                "data" : {
                    "user_name": "ashvin",
                    "password" : "ashvin",
                    "device_type":"android",
                    "app_version":"0.1",
                    "android_version":"11.0.0",
                }
            }*/
        JSONObject signUpUser = new JSONObject();
        signUpUser.AddField("user_name", registerUsernameText.text);
        signUpUser.AddField("password", registerPasswordText.text);
        signUpUser.AddField("device_type", "android");
        signUpUser.AddField("app_version", Application.version);
        signUpUser.AddField("android_version", "11.0.0");

        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "USER_REGISTER");
        eventSend.AddField("data", signUpUser);

        TestSocketIO.Instance.SendData("req", eventSend);
        yield return new WaitForSeconds(1);
        //form.AddField("referralId", registerReferralIdText.text);
        //form.AddField("mobile", registerMobNumText.text);



        /*using (UnityWebRequest www = UnityWebRequest.Post(TestSocketIO.myIp + "/api/auth/", form))
        {
            Debug.Log(form);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject registerUser = new JSONObject(www.downloadHandler.text);
                errorText.text = registerUser.GetField("error").ToString();
            }
            else
            {
                errorText.text = "Registeration Successfull ! Logging You In";
                StartCoroutine(StartLogin(registerMobNumText.text, registerPasswordText.text));
            }
        }*/
    }
    public void OnForgotPasswordButtonPressed()
    {
        errorText.text = "";
        if (forgotMobNumText.text.Length <= 0)
        {
            errorText.text = "Mobile Number Can't Be Empty.";
            return;
        }
        if (forgotOTPText.text.Length <= 0)
        {
            errorText.text = "OTP Can't Be Empty.";
            return;
        }
        if (forgotPasswordText.text.Length <= 0)
        {
            errorText.text = "Password Can't Be Empty.";
            return;
        }
        if (forgotPasswordConfirmText.text.Length <= 0)
        {
            errorText.text = "Confirm Password Can't Be Empty.";
            return;
        }
        if (forgotPasswordText.text != forgotPasswordConfirmText.text)
        {
            errorText.text = "Password and Confirm Password Cannot Be Different";
            return;
        }
        StartCoroutine(StartForgotPassword());
    }
    IEnumerator StartForgotPassword()
    {
        WWWForm form = new WWWForm();
        form.AddField("mobNumber", forgotMobNumText.text);
        form.AddField("password", forgotPasswordText.text);
        form.AddField("versionCode", Application.version);

        using (UnityWebRequest www = UnityWebRequest.Post(TestSocketIO.myIp + "/api/auth/forgotPassword", form))
        {
            Debug.Log(form);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject forgotPassword = new JSONObject(www.downloadHandler.text);
                errorText.text = forgotPassword.GetField("error").ToString();
            }
            else
            {
                errorText.text = "Password Changed ! Now Log In";
                ActivatePanelMainLogin(LoginScreenPanel.name);
                // DecryptToken(register.token);
            }
        }

    }
    public void LeaveGameClick()
    {
        NoteManager.Instance.HeaderText.text = "Confirm Exit";
        NoteManager.Instance.NoteText.text = " Do you want to Exit the game? ";
        NoteManager.Instance.OKButton.onClick.AddListener(LeaveClick);
        NoteManager.Instance.NotePanel.SetActive(true);
    }

    void LeaveClick()
    {
        Application.Quit();
    }











    #endregion

}
