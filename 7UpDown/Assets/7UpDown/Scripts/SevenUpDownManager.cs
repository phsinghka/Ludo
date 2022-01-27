using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SevenUpDownManager : MonoBehaviour
{
    public static SevenUpDownManager Instance;

    [Header("-----------------------Get Data--------------------")]
    public Image DiceImage;
    public Sprite[] DiceSpites;
    public int GetNumber1, GetNumber2;
    int[] Index = new int[6] { 0, 6, 12, 18, 24, 30 };

    public Animator DiceParent, DiceRoller;

    public Animator LeftSideWinAni, RightSideWinAni, MiddelSideWinAni;

    [Header("-----------------------Time Control--------------------")]
    public float _Time;
    public bool TimerControl;
    float TimerCounter;
    public Text TimerText;
    public GameObject TimerAnimator;
    public bool NoMoreBet, WinStart, DicePlay;
    public Text TimerTextDesc;
    public GameObject placeYourBet, stopBbetting;
    [Header("-----------------------Game Button--------------------")]
    public Button ClearBetButton;

    [Header("-----------------------Player data--------------------")]
    public Text PlayerName;
    public Text PlayerTotalAmount;
    public Image PlayerProfileImage;

    [Header("-----------------------Other Player Data--------------------")]
    public Text[] OtherPlayerName;
    public Text[] OtherPlayerTotalAmmount;
    public Image[] OtherPlayerProfileImage;

    [Header("-----------------------Last 10 Numbers--------------------")]
    public int[] Last10Numbers;
    public Image[] Last10BackgroundImage;
    public Text[] NumberText;

    [Header("-----------------------Game Text--------------------")]
    public Text TotalAmountText;
    public Text PlayerAddBetRightSideText, PlayerAddBetLeftSideText, PlayerAddBetMiddleText;
    public Text TotalAddBetRightSideText, TotalAddBetLeftSideText, TotalAddBetMiddleText;

    public int TotalAmountInTable;
    public int PlayerTotaleAmountRightSide, PlayerTotaleAmountLeftSide, PlayerTotaleAmountMiddle;
    public int TotalAmountRightSide, TotalAmountLeftSide, TotalAmountMiddle;

    [Header("------------------- Win Coins Animation ---------------------")]
    public RectTransform CenterPoint;
    CoinSevenUpDown[] Coins;
    public RectTransform[] PlayerCoinPostion;
    public GameObject[] CoinsPrefebs;

    public JSONObject WinData;

    public Animator[] WinTextAnimation;
    public Text[] WinAmountText;

    float otherAmount;

    public Color[] WinColorText;



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ClearBetButton.onClick.AddListener(ClearBetButtonClick);
        //PlayerProfileImage.sprite = MainDataManager.Instance.avatars[MainDataManager.Instance.avatarId];
        //PlayerName.text = MainDataManager.Instance.userName;
        //PlayerTotalAmount.text = MainDataManager.Instance.playPoints.ToString();
        Last10NumberFunction();

        PlayerName.text = MainDataManager.Instance.userName;
        PlayerTotalAmount.text = (MainDataManager.Instance.playPoint).ToString();
        PlayerProfileImage.sprite = MainDataManager.Instance.avatars[MainDataManager.Instance.avatarId];
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerControl)
        {
            TimerFunction();
        }

        if (PlaceBetNumberCount > 0)
        {
            Debug.Log("Place Update Call ==> ");
            SevenUpDownManager.Instance.OtherPlayerBetadd(SevenUpDownManager.Instance.datas[TestSocketIO.Instance.CurrentPlaceBetData]);
            SevenUpDownManager.Instance.datas[TestSocketIO.Instance.CurrentPlaceBetData] = null;

            PlaceBetNumberCount--;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveRoomClick();
        }

    }

    #region  Text Update Functions
    public void PlayerInfoUpdate()
    {
        PlayerTotalAmount.text = MainDataManager.Instance.playPoint.ToString();
    }

    public void TotalAmountTextUpdate()
    {
        TotalAmountInTable = (TotalAmountRightSide + TotalAmountLeftSide + TotalAmountMiddle);
        TotalAmountText.text = TotalAmountInTable.ToString();
    }

    public void TotalAddBetRightSideTextUpdate()
    {
        if (TotalAmountRightSide > 0)
        {
            TotalAddBetRightSideText.text = TotalAmountRightSide.ToString();
            TotalAddBetRightSideText.gameObject.SetActive(true);
        }
        else
        {
            TotalAddBetRightSideText.gameObject.SetActive(false);
        }

        TotalAmountTextUpdate();
    }

    public void TotalAddBetLeftSideTextUpdate()
    {
        if (TotalAmountLeftSide > 0)
        {
            TotalAddBetLeftSideText.text = TotalAmountLeftSide.ToString();
            TotalAddBetLeftSideText.gameObject.SetActive(true);
        }
        else
        {
            TotalAddBetLeftSideText.gameObject.SetActive(false);
        }

        TotalAmountTextUpdate();
    }

    public void TotalAddBetMiddleTextUpdate()
    {
        if (TotalAmountMiddle > 0)
        {
            TotalAddBetMiddleText.text = TotalAmountMiddle.ToString();
            TotalAddBetMiddleText.gameObject.SetActive(true);
        }
        else
        {
            TotalAddBetMiddleText.gameObject.SetActive(false);
        }

        TotalAmountTextUpdate();
    }

    public void PlayerAddBetRightSideTextUpdate(int Amount)
    {
        PlayerTotaleAmountRightSide += Amount;
        if (PlayerTotaleAmountRightSide > 0)
        {
            PlayerAddBetRightSideText.text = PlayerTotaleAmountRightSide.ToString();
            PlayerAddBetRightSideText.gameObject.SetActive(true);
        }
        else
        {
            PlayerAddBetRightSideText.gameObject.SetActive(false);
        }
    }

    public void PlayerAddBetLeftSideTextUpdate(int Amount)
    {
        Debug.Log("55555555555555555 === " + Amount);
        PlayerTotaleAmountLeftSide += Amount;
        if (PlayerTotaleAmountLeftSide > 0)
        {
            PlayerAddBetLeftSideText.text = PlayerTotaleAmountLeftSide.ToString();
            PlayerAddBetLeftSideText.gameObject.SetActive(true);
        }
        else
        {
            PlayerAddBetLeftSideText.gameObject.SetActive(false);
        }
    }

    public void PlayerAddBetMiddleTextUpdate(int Amount)
    {
        PlayerTotaleAmountMiddle += Amount;
        if (PlayerTotaleAmountMiddle > 0)
        {
            PlayerAddBetMiddleText.text = PlayerTotaleAmountMiddle.ToString();
            PlayerAddBetMiddleText.gameObject.SetActive(true);
        }
        else
        {
            PlayerAddBetMiddleText.gameObject.SetActive(false);
        }
    }

    #endregion

    public void ShowNumberOnDices()
    {
        // get Numbers
        int ShowImageIndex = Index[GetNumber1 - 1] + GetNumber2 - 1;
        //Debug.Log("343434343 ==> " + ShowImageIndex);

        DiceRoller.enabled = false;
        DiceImage.sprite = DiceSpites[ShowImageIndex];

        // Adding Last Number
        for (int i = 0; i < Last10Numbers.Length; i++)
        {
            if (i == Last10Numbers.Length - 1)
            {
                Last10Numbers[i] = (GetNumber1 + GetNumber2);
            }
            else
            {
                Last10Numbers[i] = Last10Numbers[i + 1];
            }
        }
        Last10NumberFunction();

        if (Last10Numbers[9] <= 6)
        {
            LeftSideWinAni.gameObject.SetActive(true);
            LeftSideWinAni.Play("WinLightAnimations");
        }
        else if (Last10Numbers[9] == 7)
        {
            MiddelSideWinAni.gameObject.SetActive(true);
            MiddelSideWinAni.Play("WinLightAnimations");
        }
        else if (Last10Numbers[9] <= 12)
        {
            RightSideWinAni.gameObject.SetActive(true);
            RightSideWinAni.Play("WinLightAnimations");
        }

        Coins = FindObjectsOfType<CoinSevenUpDown>();

        for (int i = 0; i < Coins.Length; i++)
        {
            Coins[i].MoveToCenterAni();
        }

        for (int i = 0; i < WinTextAnimation.Length; i++)
        {
            WinTextAnimation[i].Play("New State");
        }

        PlayerTotaleAmountLeftSide = 0;
        PlayerTotaleAmountMiddle = 0;
        PlayerTotaleAmountRightSide = 0;
        //Up side
        TotalAmountLeftSide = 0;
        TotalAddBetLeftSideTextUpdate();
        PlayerAddBetLeftSideTextUpdate(0);
        //Down Side
        TotalAmountRightSide = 0;
        
        TotalAddBetRightSideTextUpdate();
        PlayerAddBetRightSideTextUpdate(0);
        //Middle Side
        TotalAmountMiddle = 0;
        TotalAddBetMiddleTextUpdate();
        PlayerAddBetMiddleTextUpdate(0);

        UpStar.gameObject.SetActive(false);
        DownStar.gameObject.SetActive(false);

        TotalAmountTextUpdate();

        Invoke("WaitForWin", 1f);
    }

    public void WaitForWin()
    {
        UpdatePlayersDataOnWin(WinData);
    }

    #region TimerFunction
    bool TimerAnimationPlay;
    public void TimerFunction()
    {
        TimerCounter += Time.deltaTime;
        float time = (_Time + 1) - TimerCounter;

        TimerText.text = ((int)time).ToString();

        if (time < 4.1f)
        {
            if (!TimerAnimationPlay)
            {
                TimerAnimator.SetActive(true);
                TimerAnimationPlay = true;
            }
        }

        if (time < 1)
        {
            if (!NoMoreBet)
            {
                TimerControl = false;
                TimerCounter = 0;

                NoMoreBet = true;
                StartDicesAnimation();
                DiceParent.speed = 1;
                DiceParent.Play("DicePos");
                DiceRoller.enabled = true;
                DiceRoller.Play("New State");
                Invoke("DiseRollerAnimation", 0.5f);
                Invoke("DiseRollerAnimationFinish", 2f);
            }
            else if (WinStart)
            {
                TimerControl = false;
                TimerCounter = 0;
                WinStart = false;
                RestTimeStart();

                if (DicePlay)
                {
                    DiceParent.Play("Dicebake");
                    DicePlay = false;
                }
            }
            else if (!WinStart)
            {
                TimerControl = false;
                TimerCounter = 0;

                DiceParent.Play("New State");
                RestartNewRound();
            }

        }
    }

    public void StartDicesAnimation()
    {
        stopBbetting.SetActive(true);
        TimerTextDesc.text = "Reuslt";
        Invoke("FalseStopBet", 1.2f);
        DicePlay = true;

        _Time = 10;
        TimerControl = true;

        //Win And All
        WinStart = true;
    }

    public void RestTimeStart()
    {
        if (Last10Numbers[9] <= 6)
        {
            LeftSideWinAni.gameObject.SetActive(false);
            LeftSideWinAni.Play("New State");
        }
        else if (Last10Numbers[9] == 7)
        {
            MiddelSideWinAni.gameObject.SetActive(false);
            MiddelSideWinAni.Play("New State");
        }
        else if (Last10Numbers[9] <= 12)
        {
            RightSideWinAni.gameObject.SetActive(false);
            RightSideWinAni.Play("New State");
        }

        TimerTextDesc.text = "Rest";
        _Time = 5;
        TimerControl = true;
    }

    public void RestartNewRound()
    {
        placeYourBet.SetActive(true);
        TimerTextDesc.text = "Bet";
        Invoke("FalsePlaceYourBet", 1.2f);
        _Time = 15;
        TimerControl = true;

        TimerAnimator.SetActive(false);
        TimerAnimationPlay = false;

        NoMoreBet = false;
    }
    public void FalsePlaceYourBet()
    {
        placeYourBet.SetActive(false);

    }
    public void FalseStopBet()
    {
        stopBbetting.SetActive(false);

    }

    void DiseRollerAnimation()
    {
        DiceRoller.Play("DiceRoll");
    }

    void DiseRollerAnimationFinish()
    {
        ShowNumberOnDices();
    }



    #endregion
    public void ClearBetButtonClick()
    {
        if (SevenUpDownManager.Instance.NoMoreBet)
            return;


        Coins = FindObjectsOfType<CoinSevenUpDown>();

        for (int i = 0; i < Coins.Length; i++)
        {
            if (Coins[i].IsPlayerCoin)
            {
                Destroy(Coins[i].gameObject);
            }
        }

        JSONObject clearBet = new JSONObject();
        clearBet.AddField("userId", MainDataManager.Instance.id);
        clearBet.AddField("gameName", "sevenUp");
        TestSocketIO.Instance.SendData("clearBet", clearBet);

        //Up side
        TotalAmountLeftSide -= PlayerTotaleAmountLeftSide;
        TotalAddBetLeftSideTextUpdate();
        PlayerTotaleAmountLeftSide = 0;
        PlayerAddBetLeftSideTextUpdate(0);

        //Down Side
        TotalAmountRightSide -= PlayerTotaleAmountRightSide;
        TotalAddBetRightSideTextUpdate();
        PlayerTotaleAmountRightSide = 0;
        PlayerAddBetRightSideTextUpdate(0);

        //Middle Side
        TotalAmountMiddle -= PlayerTotaleAmountMiddle;
        TotalAddBetMiddleTextUpdate();
        PlayerTotaleAmountMiddle = 0;
        PlayerAddBetMiddleTextUpdate(0);

    }

    #region OtherPlayerInfo
    [Header("-------------------- Get Data ----------------------")]
    public float GetTime;
    public int[] Numbers;

    public OthetPlayersData[] SixPlayerData;
    public int TotalPlayerInTable;
    public void GetData(JSONObject Data)
    {
        GetTime = float.Parse(Data.GetField("time").ToString());

        string num = Data.GetField("numbers").ToString().Replace("[", "").Replace("]", "").Replace("\"", "");
        Numbers = Array.ConvertAll(num.Split(','), int.Parse);
        Array.Reverse(Numbers);

        SetTimeAndLastNumberFunction(GetTime, Numbers);

        int Count = Data.GetField("currentUsers").Count;
        TotalPlayerInTable = Count;
        playersData = Data;
        OtherPlayerDataStoreFunction(playersData.GetField("currentUsers"));


        TableTotalBetAmountFunction(Data.GetField("games"));
    }
    public void UpdatePlayersDataOnJoin(JSONObject data)
    {
        Debug.Log("22222222222 ==> " + playersData);
        Debug.Log("11111111111 ==> " + data.GetField("newUser"));
        data.GetField("newUser").AddField("wonAmount", 0);
        // playersData.AddField("currentUsers", data.GetField("newUser").GetField("_id"));
        playersData.GetField("currentUsers").AddField(data.GetField("newUser").GetField("_id").ToString().Replace("\"", ""), data.GetField("newUser"));
        Debug.Log("22222222222 ==> " + playersData);
        int Count = playersData.GetField("currentUsers").Count;
        TotalPlayerInTable = Count;
        OtherPlayerDataStoreFunction(playersData.GetField("currentUsers"));
    }
    public void UpdatePlayersDataOnLeave(JSONObject data)
    {

        Debug.Log("22222222222 ==> " + playersData);
        Debug.Log("11111111111 ==> " + data.GetField("currentUsers"));
        Debug.Log("11111111111 ==> " + data.GetField("currentUsers").ToString().Replace("\"", ""));
        playersData.RemoveField("currentUsers");
        playersData.AddField("currentUsers", data.GetField("currentUsers"));
        // if (playersData.GetField("currentUsers").HasField(data.GetField("userId").ToString().Replace("\"", "")))
        // {
        //     Debug.Log("11111111");
        //     playersData.GetField("currentUsers").RemoveField(data.GetField("userId").ToString().Replace("\"", ""));
        // }
        int Count = playersData.GetField("currentUsers").Count;
        TotalPlayerInTable = Count;
        Debug.Log("3333333333333 ==> " + playersData.GetField("currentUsers").ToString().Replace("\"{", "{").Replace("}\"", "}"));
        OtherPlayerDataStoreFunction(new JSONObject(playersData.GetField("currentUsers").ToString().Replace("\"{", "{").Replace("}\"", "}")));
    }

    public void UpdatePlayersDataOnWin(JSONObject data)
    {
        Debug.Log("Enter ====> " + data.ToString());
        otherAmount = 0;
        for (int i = 0; i < data.GetField("currentUsers").Count; i++)
        {
            if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == MainDataManager.Instance.id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[0].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[0];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().CoinAnimations = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[6].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[6].color = WinColorText[0];
                }
                else
                {
                    WinAmountText[6].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[6].color = WinColorText[1];
                }

                MainDataManager.Instance.playPoint = int.Parse(data.GetField("currentUsers")[i].GetField("playPoint").ToString());
                PlayerTotalAmount.text = (MainDataManager.Instance.playPoint).ToString();

            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[0].Id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[1].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[1];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[0].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[0].color = WinColorText[0];
                }
                else
                {
                    WinAmountText[0].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[0].color = WinColorText[1];
                }

            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[1].Id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[2].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[2];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[1].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[1].color = WinColorText[0];
                }
                else
                {
                    WinAmountText[1].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[1].color = WinColorText[1];
                }
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[2].Id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[3].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[3];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[2].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[2].color = WinColorText[0];
                }
                else
                {
                    WinAmountText[2].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[2].color = WinColorText[1];
                }

            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[3].Id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[4].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[4];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[3].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[3].color = WinColorText[0];
                }
                else
                {
                    WinAmountText[3].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[3].color = WinColorText[1];
                }

            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[4].Id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[5].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[5];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[4].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[4].color = WinColorText[0];
                }
                else
                {
                    WinAmountText[4].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[4].color = WinColorText[1];
                }

            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[5].Id)
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[6].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[6];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    WinAmountText[5].text = "+" + data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[5].color = WinColorText[0];   
                }
                else
                {
                    WinAmountText[5].text = data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString();
                    WinAmountText[5].color = WinColorText[1];   
                }
            }
            else
            {
                if (int.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString()) > 0)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = CenterPoint.transform.position;
                        GameObject Prefeb = Instantiate(CoinsPrefebs[UnityEngine.Random.Range(0, CoinsPrefebs.Length)], pos, Quaternion.identity);
                        Prefeb.transform.parent = PlayerCoinPostion[7].gameObject.transform;
                        Prefeb.transform.localScale = new Vector3(1, 1, 1);
                        Prefeb.GetComponent<CoinSevenUpDown>().TO = PlayerCoinPostion[7];
                        Prefeb.GetComponent<CoinSevenUpDown>().WinTimeAni = true;
                        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = CenterPoint.gameObject;
                    }
                    otherAmount += float.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString());
                    
                }
                else
                {
                    otherAmount += float.Parse(data.GetField("currentUsers")[i].GetField("currentWonAmount").ToString());

                }
            }
        }

        if (otherAmount > 0)
        {
            WinAmountText[7].text = "+" + otherAmount.ToString();
            WinAmountText[7].color = WinColorText[0];
        }
        else
        {
            WinAmountText[7].text = otherAmount.ToString();
            WinAmountText[7].color = WinColorText[1];
        }
        for (int i = 0; i < data.GetField("currentUsers").Count; i++)
        {
            if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == MainDataManager.Instance.id)
            {
                WinTextAnimation[6].Play("WinTextAnimation");
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[0].Id)
            {
                WinTextAnimation[0].Play("WinTextAnimation");
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[1].Id)
            {
                WinTextAnimation[1].Play("WinTextAnimation");
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[2].Id)
            {
                WinTextAnimation[2].Play("WinTextAnimation");
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[3].Id)
            {
                WinTextAnimation[3].Play("WinTextAnimation");
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[4].Id)
            {
                WinTextAnimation[4].Play("WinTextAnimation");
            }
            else if (data.GetField("currentUsers")[i].GetField("_id").ToString().Replace("\"", "") == SixPlayerData[5].Id)
            {
                WinTextAnimation[5].Play("WinTextAnimation");
            }
            else
            {
                WinTextAnimation[7].Play("WinTextAnimation");
            }
        }



        Debug.Log("22222222222 ==> " + playersData);
        Debug.Log("11111111111 ==> " + data.GetField("currentUsers"));
        Debug.Log("11111111111 ==> " + data.GetField("currentUsers"));
        playersData.RemoveField("currentUsers");
        playersData.AddField("currentUsers", data.GetField("currentUsers"));
        Debug.Log("3333333333333 ==> " + playersData.GetField("currentUsers").ToString().Replace("\"{", "{").Replace("}\"", "}"));
        OtherPlayerDataStoreFunction(new JSONObject(playersData.GetField("currentUsers").ToString().Replace("\"{", "{").Replace("}\"", "}")));

        WinData.Clear();
    }
    void OnApplicationQuit()
    {
        LeaveRoomButton();
    }
    void OnApplicationPause()
    {
        LeaveRoomButton();

    }
    public void LeaveRoomButton()
    {
        JSONObject joinUser = new JSONObject();
        joinUser.AddField("userId", MainDataManager.Instance.id);
        joinUser.AddField("gameName", "sevenUp");
        TestSocketIO.Instance.SendData("leaveRoom", joinUser);
        SceneManager.LoadScene("MainLogin");
    }

    public void LeaveRoomClick()
    {
        NoteManager.Instance.HeaderText.text = "Confirm Exit";
        NoteManager.Instance.NoteText.text = " Do you want to Leave room? ";
        NoteManager.Instance.OKButton.onClick.AddListener(LeaveRoomButton);
        NoteManager.Instance.NotePanel.SetActive(true);
    }

    public int[] timeValue = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    public void SetTimeAndLastNumberFunction(float time, int[] numbers)
    {
        if (time < 10)
        {
            //win time
            TimerCounter = timeValue[(int)time];
            DicePlay = false;
            _Time = 10;
            NoMoreBet = true;
            WinStart = true;
            TimerAnimationPlay = true;
        }
        else if (time < 15)
        {
            //rest time
            TimerCounter = timeValue[(int)time];

            _Time = 5;
            NoMoreBet = true;
            WinStart = false;
            TimerAnimationPlay = true;
        }
        else if (time < 30)
        {
            // in game
            TimerCounter = timeValue[(int)time];

            _Time = 15;
        }

        // Last 10 Number set
        for (int i = 0; i < numbers.Length; i++)
        {
            Last10Numbers[i] = numbers[i];
        }
        Last10NumberFunction();
    }
    public JSONObject playersData;
    public Text TotalPlayerText;
    public void OtherPlayerDataStoreFunction(JSONObject Data)
    {
        if (TotalPlayerInTable > 7)
        {
            TotalPlayerText.text = TotalPlayerInTable + " Players";
        }
        else
        {
            TotalPlayerText.text = "7  Players";
        }
        Debug.Log("Enetr ==> ");


        for (int i = 0; i < SixPlayerData.Length; i++)
        {
            SixPlayerData[i].playPoint = 0;
            SixPlayerData[i].Id = "";
            SixPlayerData[i].Name = "";
            SixPlayerData[i].playPoint = 0;
            SixPlayerData[i].winPoint = 0;
        }

        SixPlayerData[3].winPoint = -100000;

        for (int i = 0; i < TotalPlayerInTable; i++)
        {

            Debug.Log("Enter " + MainDataManager.Instance.id + "  != " + Data[i].GetField("_id").ToString().Replace("\"", "") + "  " + int.Parse(Data[i].GetField("wonAmount").ToString()));

            if (MainDataManager.Instance.id != Data[i].GetField("_id").ToString().Replace("\"", ""))
            {
                Debug.Log("Enter @@@@@@@@@@@@@");
                if (SixPlayerData[3].winPoint <= int.Parse(Data[i].GetField("wonAmount").ToString()))
                {
                    Debug.Log("Enterasfdasdf @@@@@@@@@@@@@");

                    if (SixPlayerData[3].Id != "")
                    {
                        if (SixPlayerData[0].Id == "")
                        {
                            SixPlayerData[0].Id = SixPlayerData[3].Id;
                            SixPlayerData[0].Name = SixPlayerData[3].Name;
                            SixPlayerData[0].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[0].AvatarId = SixPlayerData[3].AvatarId;
                        }
                        else if (SixPlayerData[1].Id == "")
                        {
                            SixPlayerData[1].Id = SixPlayerData[3].Id;
                            SixPlayerData[1].Name = SixPlayerData[3].Name;
                            SixPlayerData[1].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[1].AvatarId = SixPlayerData[3].AvatarId;
                        }
                        else if (SixPlayerData[4].Id == "")
                        {
                            SixPlayerData[4].Id = SixPlayerData[3].Id;
                            SixPlayerData[4].Name = SixPlayerData[3].Name;
                            SixPlayerData[4].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[4].AvatarId = SixPlayerData[3].AvatarId;
                        }
                        else if (SixPlayerData[5].Id == "")
                        {
                            SixPlayerData[5].Id = SixPlayerData[3].Id;
                            SixPlayerData[5].Name = SixPlayerData[3].Name;
                            SixPlayerData[5].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[5].AvatarId = SixPlayerData[3].AvatarId;
                        }

                    }
                    SixPlayerData[3].Id = Data[i].GetField("_id").ToString().Replace("\"", "");
                    SixPlayerData[3].Name = Data[i].GetField("userName").ToString().Replace("\"", "");
                    SixPlayerData[3].playPoint = int.Parse(Data[i].GetField("playPoint").ToString().Replace("\"", ""));
                    SixPlayerData[3].AvatarId = int.Parse(Data[i].GetField("avatarId").ToString().Replace("\"", ""));
                    SixPlayerData[3].winPoint = int.Parse(Data[i].GetField("wonAmount").ToString().Replace("\"", ""));

                }
                else if (SixPlayerData[2].playPoint <= int.Parse(Data[i].GetField("playPoint").ToString()))
                {
                    if (SixPlayerData[2].Id != "")
                    {
                        if (SixPlayerData[0].Id == "")
                        {
                            SixPlayerData[0].Id = SixPlayerData[3].Id;
                            SixPlayerData[0].Name = SixPlayerData[3].Name;
                            SixPlayerData[0].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[0].AvatarId = SixPlayerData[3].AvatarId;
                        }
                        else if (SixPlayerData[1].Id == "")
                        {
                            SixPlayerData[1].Id = SixPlayerData[3].Id;
                            SixPlayerData[1].Name = SixPlayerData[3].Name;
                            SixPlayerData[1].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[1].AvatarId = SixPlayerData[3].AvatarId;
                        }
                        else if (SixPlayerData[4].Id == "")
                        {
                            SixPlayerData[4].Id = SixPlayerData[3].Id;
                            SixPlayerData[4].Name = SixPlayerData[3].Name;
                            SixPlayerData[4].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[4].AvatarId = SixPlayerData[3].AvatarId;
                        }
                        else if (SixPlayerData[5].Id == "")
                        {
                            SixPlayerData[5].Id = SixPlayerData[3].Id;
                            SixPlayerData[5].Name = SixPlayerData[3].Name;
                            SixPlayerData[5].playPoint = SixPlayerData[3].playPoint;
                            SixPlayerData[5].AvatarId = SixPlayerData[3].AvatarId;
                        }

                    }
                    SixPlayerData[2].Id = Data[i].GetField("_id").ToString().Replace("\"", "");
                    SixPlayerData[2].Name = Data[i].GetField("userName").ToString().Replace("\"", "");
                    SixPlayerData[2].playPoint = int.Parse(Data[i].GetField("playPoint").ToString().Replace("\"", ""));
                    SixPlayerData[2].AvatarId = int.Parse(Data[i].GetField("avatarId").ToString().Replace("\"", ""));
                }
            }
        }


        Debug.Log("Enter 8888");
        if (SixPlayerData[0].Id == "")
        {
            SixPlayerData[0].Id = "dummy";
            SixPlayerData[0].Name = "Guest" + UnityEngine.Random.Range(0, 1020).ToString("0000");
            SixPlayerData[0].playPoint = UnityEngine.Random.Range(10000, 1000000);
            SixPlayerData[0].AvatarId = UnityEngine.Random.Range(0, 10);
        }
        if (SixPlayerData[1].Id == "")
        {
            SixPlayerData[1].Id = "dummy";
            SixPlayerData[1].Name = "Guest" + UnityEngine.Random.Range(0, 1020).ToString("0000");
            SixPlayerData[1].playPoint = UnityEngine.Random.Range(10000, 1000000);
            SixPlayerData[1].AvatarId = UnityEngine.Random.Range(0, 10);
        }
        if (SixPlayerData[2].Id == "")
        {
            SixPlayerData[2].Id = "dummy";
            SixPlayerData[2].Name = "Guest" + UnityEngine.Random.Range(0, 1020).ToString("0000");
            SixPlayerData[2].playPoint = UnityEngine.Random.Range(10000, 1000000);
            SixPlayerData[2].AvatarId = UnityEngine.Random.Range(0, 10);
        }
        if (SixPlayerData[3].Id == "")
        {
            SixPlayerData[3].Id = "dummy";
            SixPlayerData[3].Name = "Guest" + UnityEngine.Random.Range(0, 1020).ToString("0000");
            SixPlayerData[3].playPoint = UnityEngine.Random.Range(10000, 1000000);
            SixPlayerData[3].AvatarId = UnityEngine.Random.Range(0, 10);
        }
        if (SixPlayerData[4].Id == "")
        {
            SixPlayerData[4].Id = "dummy";
            SixPlayerData[4].Name = "Guest" + UnityEngine.Random.Range(0, 1020).ToString("0000");
            SixPlayerData[4].playPoint = UnityEngine.Random.Range(10000, 1000000);
            SixPlayerData[4].AvatarId = UnityEngine.Random.Range(0, 10);
        }
        if (SixPlayerData[5].Id == "")
        {
            SixPlayerData[5].Id = "dummy";
            SixPlayerData[5].Name = "Guest" + UnityEngine.Random.Range(0, 1020).ToString("0000");
            SixPlayerData[5].playPoint = UnityEngine.Random.Range(10000, 1000000);
            SixPlayerData[5].AvatarId = UnityEngine.Random.Range(0, 10);
        }

        for (int i = 0; i < OtherPlayerName.Length; i++)
        {
            OtherPlayerName[i].text = SixPlayerData[i].Name;
            OtherPlayerTotalAmmount[i].text = (SixPlayerData[i].playPoint).ToString();
            OtherPlayerProfileImage[i].sprite = MainDataManager.Instance.avatars[SixPlayerData[i].AvatarId];
        }
    }

    int[] CoinsAountsUp;
    int[] CoinsUpIndex;


    public void TableTotalBetAmountFunction(JSONObject Data)
    {
        TotalAmountLeftSide = int.Parse(Data.GetField("totalUp").ToString());
        TotalAmountRightSide = int.Parse(Data.GetField("totalDown").ToString());
        TotalAmountMiddle = int.Parse(Data.GetField("totalMiddle").ToString());

        TotalAddBetLeftSideTextUpdate();
        TotalAddBetRightSideTextUpdate();
        TotalAddBetMiddleTextUpdate();

        PlayerAddBetLeftSideTextUpdate(int.Parse(Data.GetField("up").ToString()));
        PlayerAddBetRightSideTextUpdate(int.Parse(Data.GetField("down").ToString()));
        PlayerAddBetMiddleTextUpdate(int.Parse(Data.GetField("middle").ToString()));

        int K;
        int TotalUpStore = 0;
        int Count = 0;

        if (TotalAmountLeftSide > 0)
        {
            TotalUpStore = TotalAmountLeftSide;

            if (TotalAmountLeftSide < 100)
            {
                K = TotalAmountLeftSide / 10;
            }
            else if (TotalAmountLeftSide < 1000)
            {
                K = TotalAmountLeftSide / 100;
                Debug.Log("11111111111");
            }
            else if (TotalAmountLeftSide < 5000)
            {
                K = TotalAmountLeftSide / 100;
            }
            else
            {
                K = TotalAmountLeftSide / 300;
            }
            CoinsAountsUp = new int[K];
            CoinsUpIndex = new int[K];
            for (int i = 0; i < K; i++)
            {
                if (TotalUpStore > 0)
                {
                    if (TotalUpStore > 10000)
                    {
                        TotalUpStore -= 5000;
                        CoinsUpIndex[i] = 5;
                        Count++;
                    }
                    else if (TotalUpStore > 5000)
                    {
                        TotalUpStore -= 1000;
                        CoinsUpIndex[i] = 4;
                        Count++;
                    }
                    else if (TotalUpStore > 500)
                    {
                        TotalUpStore -= 500;
                        CoinsUpIndex[i] = 3;
                        Count++;
                    }
                    else if (TotalUpStore > 100)
                    {
                        TotalUpStore -= 100;
                        CoinsUpIndex[i] = 2;
                        Count++;
                    }
                    else if (TotalUpStore > 10)
                    {
                        TotalUpStore -= 10;
                        CoinsUpIndex[i] = 0;
                        Count++;

                        Debug.Log("222222222222");
                    }
                }
            }

            for (int i = 0; i < Count; i++)
            {

                Debug.Log("333333333333333");
                Vector3 pos = new Vector3(UnityEngine.Random.Range(Sides[0].rect.xMin, Sides[0].rect.xMax), UnityEngine.Random.Range(Sides[0].rect.yMin, Sides[0].rect.yMax), 0) + Sides[0].transform.position;
                GameObject Prefeb = Instantiate(CoinPrefebsOther[CoinsUpIndex[i]], pos, Quaternion.identity);
                Prefeb.transform.parent = Sides[0].gameObject.transform;
                Prefeb.transform.localScale = new Vector3(1, 1, 1);
                Prefeb.GetComponent<CoinSevenUpDown>().PlayTimeAdd = true;
                Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
                Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[0];
            }

            K = 0;
            TotalUpStore = 0;
            Count = 0;
        }

        if (TotalAmountRightSide > 0)
        {
            TotalUpStore = TotalAmountRightSide;

            if (TotalAmountRightSide < 100)
            {
                K = TotalAmountRightSide / 10;
            }
            else if (TotalAmountRightSide < 1000)
            {
                K = TotalAmountRightSide / 100;
            }
            else if (TotalAmountRightSide < 5000)
            {
                K = TotalAmountRightSide / 100;
            }
            else
            {
                K = TotalAmountRightSide / 300;
            }
            CoinsAountsUp = new int[K];
            CoinsUpIndex = new int[K];
            for (int i = 0; i < K; i++)
            {
                if (TotalUpStore > 0)
                {
                    if (TotalUpStore > 10000)
                    {
                        TotalUpStore -= 5000;
                        CoinsUpIndex[i] = 5;
                        Count++;
                    }
                    else if (TotalUpStore > 5000)
                    {
                        TotalUpStore -= 1000;
                        CoinsUpIndex[i] = 4;
                        Count++;
                    }
                    else if (TotalUpStore > 500)
                    {
                        TotalUpStore -= 500;
                        CoinsUpIndex[i] = 3;
                        Count++;
                    }
                    else if (TotalUpStore > 100)
                    {
                        TotalUpStore -= 100;
                        CoinsUpIndex[i] = 2;
                        Count++;
                    }
                    else if (TotalUpStore > 10)
                    {
                        TotalUpStore -= 10;
                        CoinsUpIndex[i] = 0;
                        Count++;
                    }
                }
            }

            for (int i = 0; i < Count; i++)
            {
                Vector3 pos = new Vector3(UnityEngine.Random.Range(Sides[2].rect.xMin, Sides[2].rect.xMax), UnityEngine.Random.Range(Sides[2].rect.yMin, Sides[2].rect.yMax), 0) + Sides[2].transform.position;
                GameObject Prefeb = Instantiate(CoinPrefebsOther[CoinsUpIndex[i]], pos, Quaternion.identity);
                Prefeb.transform.parent = Sides[2].gameObject.transform;
                Prefeb.transform.localScale = new Vector3(1, 1, 1);
                Prefeb.GetComponent<CoinSevenUpDown>().PlayTimeAdd = true;
                Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
                Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[0];
            }
            K = 0;
            TotalUpStore = 0;
            Count = 0;
        }

        if (TotalAmountMiddle > 0)
        {
            TotalUpStore = TotalAmountMiddle;

            if (TotalAmountMiddle < 100)
            {
                K = TotalAmountMiddle / 10;
            }
            else if (TotalAmountMiddle < 1000)
            {
                K = TotalAmountMiddle / 100;
            }
            else if (TotalAmountMiddle < 5000)
            {
                K = TotalAmountMiddle / 100;
            }
            else
            {
                K = TotalAmountMiddle / 300;
            }
            CoinsAountsUp = new int[K];
            CoinsUpIndex = new int[K];
            for (int i = 0; i < K; i++)
            {
                if (TotalUpStore > 0)
                {
                    if (TotalUpStore > 10000)
                    {
                        TotalUpStore -= 5000;
                        CoinsUpIndex[i] = 5;
                        Count++;
                    }
                    else if (TotalUpStore > 5000)
                    {
                        TotalUpStore -= 1000;
                        CoinsUpIndex[i] = 4;
                        Count++;
                    }
                    else if (TotalUpStore > 500)
                    {
                        TotalUpStore -= 500;
                        CoinsUpIndex[i] = 3;
                        Count++;
                    }
                    else if (TotalUpStore > 100)
                    {
                        TotalUpStore -= 100;
                        CoinsUpIndex[i] = 2;
                        Count++;
                    }
                    else if (TotalUpStore > 10)
                    {
                        TotalUpStore -= 10;
                        CoinsUpIndex[i] = 0;
                        Count++;
                    }
                }
            }

            for (int i = 0; i < Count; i++)
            {
                Vector3 pos = new Vector3(UnityEngine.Random.Range(Sides[1].rect.xMin, Sides[1].rect.xMax), UnityEngine.Random.Range(Sides[1].rect.yMin, Sides[1].rect.yMax), 0) + Sides[1].transform.position;
                GameObject Prefeb = Instantiate(CoinPrefebsOther[CoinsUpIndex[i]], pos, Quaternion.identity);
                Prefeb.transform.parent = Sides[1].gameObject.transform;
                Prefeb.transform.localScale = new Vector3(1, 1, 1);
                Prefeb.GetComponent<CoinSevenUpDown>().PlayTimeAdd = true;
                Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
                Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[0];
            }
            K = 0;
            TotalUpStore = 0;
            Count = 0;
        }
    }

    [Header("-----------------Other Player Bet System ----------------")]
    public RectTransform[] Sides;
    public GameObject[] CoinPrefebsOther;
    public GameObject[] OtherPlayerPostion;
    public int Count, SideCount;
    public int PlaceBetNumberCount;
    bool UP, DOWN, MIDDLE;
    public Image UpStar , DownStar;
    public GameObject UpStarAnimation , DownStarAnimation;
    public JSONObject[] datas = new JSONObject[100];
    public void OtherPlayerBetadd(JSONObject data)
    {
        Debug.Log("Place Function Call ==> ");
        if (SevenUpDownManager.Instance.NoMoreBet)
            return;

        UP = DOWN = MIDDLE = false;
        if (int.Parse(data.GetField("up").ToString()) == 10 || int.Parse(data.GetField("down").ToString()) == 10 || int.Parse(data.GetField("middle").ToString()) == 10)
        {
            Count = 0;
        }
        else if (int.Parse(data.GetField("up").ToString()) == 50 || int.Parse(data.GetField("down").ToString()) == 50 || int.Parse(data.GetField("middle").ToString()) == 50)
        {
            Count = 1;
        }
        else if (int.Parse(data.GetField("up").ToString()) == 100 || int.Parse(data.GetField("down").ToString()) == 100 || int.Parse(data.GetField("middle").ToString()) == 100)
        {
            Count = 1;
        }
        else if (int.Parse(data.GetField("up").ToString()) == 500 || int.Parse(data.GetField("down").ToString()) == 500 || int.Parse(data.GetField("middle").ToString()) == 500)
        {
            Count = 3;
        }
        else if (int.Parse(data.GetField("up").ToString()) == 1000 || int.Parse(data.GetField("down").ToString()) == 1000 || int.Parse(data.GetField("middle").ToString()) == 1000)
        {
            Count = 4;
        }
        else if (int.Parse(data.GetField("up").ToString()) == 5000 || int.Parse(data.GetField("down").ToString()) == 5000 || int.Parse(data.GetField("middle").ToString()) == 5000)
        {
            Count = 5;
        }
        else if (int.Parse(data.GetField("up").ToString()) == 10000 || int.Parse(data.GetField("down").ToString()) == 10000 || int.Parse(data.GetField("middle").ToString()) == 10000)
        {
            Count = 6;
        }

        if (int.Parse(data.GetField("up").ToString()) > 0)
        {
            UP = true;
            SideCount = 0;
            // Debug.Log("upppppppppppppppppppppp");
            TotalAmountLeftSide += int.Parse(data.GetField("up").ToString());
            TotalAddBetLeftSideTextUpdate();
        }
        else if (int.Parse(data.GetField("down").ToString()) > 0)
        {
            DOWN = true;
            SideCount = 1;
            // Debug.Log("Downnnnnnnnnnnnnnnnnnn");
            TotalAmountRightSide += int.Parse(data.GetField("down").ToString());
            TotalAddBetRightSideTextUpdate();
        }
        else if (int.Parse(data.GetField("middle").ToString()) > 0)
        {
            MIDDLE = true;
            SideCount = 2;
            // Debug.Log("Middleeeeeeeeeeeeeeeeeee");
            TotalAmountMiddle += int.Parse(data.GetField("middle").ToString());
            TotalAddBetMiddleTextUpdate();
        }

        if (data.GetField("userId").ToString().Replace("\"", "") == SixPlayerData[0].Id)
        {
            Vector3 pos = OtherPlayerPostion[0].transform.position;
            GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
            Prefeb.transform.parent = Sides[SideCount].gameObject.transform;
            Prefeb.transform.localScale = new Vector3(1, 1, 1);
            // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

            Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[SideCount].gameObject;
            Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
            Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[SideCount];
            Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[0];
            Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        }
        else if (data.GetField("userId").ToString().Replace("\"", "") == SixPlayerData[1].Id)
        {
            Vector3 pos = OtherPlayerPostion[1].transform.position;
            GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
            Prefeb.transform.parent = Sides[SideCount].gameObject.transform;
            Prefeb.transform.localScale = new Vector3(1, 1, 1);
            // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

            Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[SideCount].gameObject;
            Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
            Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[SideCount];
            Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[1];
            Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        }
        else if (data.GetField("userId").ToString().Replace("\"", "") == SixPlayerData[2].Id)
        {
            Vector3 pos = OtherPlayerPostion[2].transform.position;
            GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
            Prefeb.transform.parent = Sides[SideCount].gameObject.transform;
            Prefeb.transform.localScale = new Vector3(1, 1, 1);
            // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

            Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[SideCount].gameObject;
            Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
            Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[SideCount];
            Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[2];
            Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        }
        else if (data.GetField("userId").ToString().Replace("\"", "") == SixPlayerData[3].Id)
        {
            if(int.Parse(data.GetField("up").ToString()) > 0)
            {
                UpStarAnimation.SetActive(true);
                Invoke("LeftStarAniRemove" , 0.5f);

            }
            else if(int.Parse(data.GetField("down").ToString()) > 0)
            {
                DownStarAnimation.SetActive(true);
                Invoke("RightStarAniRemove" , 0.5f);

            }
            else if(int.Parse(data.GetField("middle").ToString()) > 0)
            {
                Vector3 pos = OtherPlayerPostion[3].transform.position;
                GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
                Prefeb.transform.parent = Sides[1].gameObject.transform;
                Prefeb.transform.localScale = new Vector3(1, 1, 1);
                // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

                Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[1].gameObject;
                Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
                Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[1];
                Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[3];
                Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
            }
          
        }
        else if (data.GetField("userId").ToString().Replace("\"", "") == SixPlayerData[4].Id)
        {
            Vector3 pos = OtherPlayerPostion[4].transform.position;
            GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
            Prefeb.transform.parent = Sides[SideCount].gameObject.transform;
            Prefeb.transform.localScale = new Vector3(1, 1, 1);
            // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

            Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[SideCount].gameObject;
            Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
            Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[SideCount];
            Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[4];
            Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        }
        else if (data.GetField("userId").ToString().Replace("\"", "") == SixPlayerData[5].Id)
        {
            Vector3 pos = OtherPlayerPostion[5].transform.position;
            GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
            Prefeb.transform.parent = Sides[SideCount].gameObject.transform;
            Prefeb.transform.localScale = new Vector3(1, 1, 1);
            // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

            Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[SideCount].gameObject;
            Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
            Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[SideCount];
            Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[5];
            Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        }
        else
        {
            Vector3 pos = OtherPlayerPostion[6].transform.position;
            GameObject Prefeb = Instantiate(CoinPrefebsOther[Count], pos, Quaternion.identity);
            Prefeb.transform.parent = Sides[SideCount].gameObject.transform;
            Prefeb.transform.localScale = new Vector3(1, 1, 1);
            // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

            Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = Sides[SideCount].gameObject;
            Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = false;
            Prefeb.GetComponent<CoinSevenUpDown>().TO = Sides[SideCount];
            Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = OtherPlayerPostion[6];
            Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        }
    }


    #endregion

    #region Last10NumberFunction
    //Last 10 Number Updateing Function
    public void Last10NumberFunction()
    {
        for (int i = 0; i < Last10Numbers.Length; i++)
        {
            if (Last10Numbers[i] <= 6)
            {
                Last10BackgroundImage[i].color = new Color(0.8301887f, 0.3250267f, 0.3955144f, 1);
                NumberText[i].text = Last10Numbers[i].ToString();
            }
            else if (Last10Numbers[i] == 7)
            {
                Last10BackgroundImage[i].color = new Color(0.1294118f, 0.482353f, 0.6784314f, 1);
                NumberText[i].text = Last10Numbers[i].ToString();
            }
            else if (Last10Numbers[i] > 7)
            {
                Last10BackgroundImage[i].color = new Color(0.07058824f, 0.7843138f, 0.1960784f, 1);
                NumberText[i].text = Last10Numbers[i].ToString();
            }
        }
    }

    #endregion

    public void LeftStarAniRemove()
    {
        UpStar.gameObject.SetActive(true);
        UpStarAnimation.SetActive(false);
    }
    public void RightStarAniRemove()
    {
        DownStar.gameObject.SetActive(true);
        DownStarAnimation.SetActive(false);
    }

}

[Serializable]
public class OthetPlayersData
{
    public string Id;
    public string Name;
    public int playPoint;
    public int winPoint;
    public int AvatarId;

}